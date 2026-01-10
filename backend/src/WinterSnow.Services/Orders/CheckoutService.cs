using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Core.Domain.Payments;
using WinterSnow.Core.Domain.Customers;
using WinterSnow.Data;
using WinterSnow.Services.Payments;

namespace WinterSnow.Services.Orders;

public class CheckoutService : ICheckoutService
{
    private readonly WinterSnowDbContext _db;
    private readonly IAddressValidationService _addressValidation;
    private readonly ICashfreeGateway _cashfree;

    public CheckoutService(
        WinterSnowDbContext db,
        IAddressValidationService addressValidation,
        ICashfreeGateway cashfree)
    {
        _db = db;
        _addressValidation = addressValidation;
        _cashfree = cashfree;
    }

    public async Task<List<SplitOrderSummary>> PreviewSplitAsync(List<CheckoutItem> items, CancellationToken ct = default)
    {
        var normalized = items
            .Where(i => i.Quantity > 0)
            .GroupBy(i => new { i.ProductId, i.VariantId })
            .Select(g => new CheckoutItem { ProductId = g.Key.ProductId, VariantId = g.Key.VariantId, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        var productIds = normalized.Select(x => x.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => p.Published && p.IsApprovedByAdmin && productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        var variantIds = normalized.Where(x => x.VariantId is not null).Select(x => x.VariantId!.Value).Distinct().ToList();
        var variants = await _db.ProductVariants
            .Where(v => variantIds.Contains(v.Id))
            .ToDictionaryAsync(v => v.Id, ct);

        var result = new Dictionary<int, SplitOrderSummary>();

        foreach (var item in normalized)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                continue;

            decimal unitPrice = product.Price;
            if (item.VariantId is not null && variants.TryGetValue(item.VariantId.Value, out var variant) && variant.OverridePrice is not null)
                unitPrice = variant.OverridePrice.Value;

            if (!result.TryGetValue(product.VendorId, out var summary))
            {
                summary = new SplitOrderSummary { VendorId = product.VendorId, Currency = product.Currency };
                result[product.VendorId] = summary;
            }

            summary.Items.Add(new SplitOrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });
        }

        foreach (var s in result.Values)
        {
            s.Subtotal = s.Items.Sum(i => i.UnitPrice * i.Quantity);
            // Starter logic: flat shipping per vendor
            var shipping = s.Subtotal > 999 ? 0 : 79;
            s.OrderTotal = s.Subtotal + shipping;
        }

        return result.Values.OrderBy(x => x.VendorId).ToList();
    }

    public async Task<CheckoutResult> CreateOrdersAndPaymentSessionAsync(int customerId, CheckoutRequest request, CancellationToken ct = default)
    {
        var addressValidation = await _addressValidation.ValidateAsync(request.ShippingAddress, ct);
        if (!addressValidation.IsValid || addressValidation.Normalized is null)
            throw new InvalidOperationException(addressValidation.Message ?? "Invalid address.");

        var split = await PreviewSplitAsync(request.Items, ct);
        if (split.Count == 0)
            throw new InvalidOperationException("Cart is empty or contains invalid items.");

        var customer = await _db.Users.FirstOrDefaultAsync(u => u.Id == customerId && u.UserType == UserType.Customer, ct);
        if (customer is null)
            throw new InvalidOperationException("Customer not found.");

        // Inventory check (Stock Guard)
        var requestedVariants = request.Items.Where(i => i.VariantId is not null).ToList();
        var variantIds = requestedVariants.Select(i => i.VariantId!.Value).Distinct().ToList();
        var variants = await _db.ProductVariants.Where(v => variantIds.Contains(v.Id)).ToDictionaryAsync(v => v.Id, ct);

        foreach (var i in requestedVariants)
        {
            if (!variants.TryGetValue(i.VariantId!.Value, out var v))
                throw new InvalidOperationException($"Variant {i.VariantId} not found.");
            if (i.Quantity <= 0)
                throw new InvalidOperationException("Invalid quantity.");
            if (v.StockQuantity < i.Quantity)
                throw new InvalidOperationException($"Insufficient stock for variant {v.Id}.");
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var address = new Address
        {
            FullName = addressValidation.Normalized.FullName,
            Line1 = addressValidation.Normalized.Line1,
            Line2 = addressValidation.Normalized.Line2,
            City = addressValidation.Normalized.City,
            State = addressValidation.Normalized.State,
            PostalCode = addressValidation.Normalized.PostalCode,
            CountryCode = addressValidation.Normalized.CountryCode,
            Phone = addressValidation.Normalized.Phone
        };
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(ct);

        var createdOrderIds = new List<int>();

        // Create one order per vendor (OOM split-order logic)
        foreach (var vendorSplit in split)
        {
            var order = new Order
            {
                CustomerId = customerId,
                VendorId = vendorSplit.VendorId,
                ShippingAddressId = address.Id,
                Subtotal = vendorSplit.Subtotal,
                ShippingFee = vendorSplit.OrderTotal - vendorSplit.Subtotal,
                TaxTotal = 0,
                OrderTotal = vendorSplit.OrderTotal,
                Currency = vendorSplit.Currency,
                Status = OrderStatus.Pending
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);
            createdOrderIds.Add(order.Id);

            foreach (var line in vendorSplit.Items)
            {
                var cartLine = request.Items.FirstOrDefault(x => x.ProductId == line.ProductId);
                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = line.ProductId,
                    ProductVariantId = cartLine?.VariantId,
                    ProductName = line.ProductName ?? $"Product {line.ProductId}",
                    UnitPriceInclTax = line.UnitPrice,
                    Quantity = line.Quantity,
                    PriceInclTax = line.UnitPrice * line.Quantity
                });

                // Stock decrement
                if (cartLine?.VariantId is not null && variants.TryGetValue(cartLine.VariantId.Value, out var v))
                    v.StockQuantity -= line.Quantity;
            }

            // Ledger entries: vendor credit (net) and platform commission
            var vendor = await _db.Vendors.FirstOrDefaultAsync(v => v.Id == order.VendorId, ct);
            var commissionRate = vendor?.CommissionRate ?? 0.15m;
            var commission = Math.Round(order.Subtotal * commissionRate, 2);
            var net = order.Subtotal - commission;

            _db.VendorLedgerEntries.Add(new VendorLedgerEntry
            {
                VendorId = order.VendorId,
                OrderId = order.Id,
                Amount = net,
                Currency = order.Currency,
                Description = $"Sale credit (net of {commissionRate:P0} commission)"
            });
            _db.VendorLedgerEntries.Add(new VendorLedgerEntry
            {
                VendorId = order.VendorId,
                OrderId = order.Id,
                Amount = -commission,
                Currency = order.Currency,
                Description = "Platform commission"
            });

            _db.PaymentTransactions.Add(new PaymentTransaction
            {
                OrderId = order.Id,
                Provider = "Cashfree",
                Status = PaymentStatus.Pending,
                Amount = order.OrderTotal,
                Currency = order.Currency
            });
        }

        await _db.SaveChangesAsync(ct);

        var totalAmount = split.Sum(s => s.OrderTotal);
        var paymentSessionId = await _cashfree.CreatePaymentSessionAsync(totalAmount, "INR", customer.Email, ct);

        await tx.CommitAsync(ct);

        return new CheckoutResult
        {
            CreatedOrderIds = createdOrderIds,
            SplitSummary = split,
            PaymentSessionId = paymentSessionId
        };
    }
}

