using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Core.Domain.Payments;
using WinterSnow.Core.Domain.Customers;
using WinterSnow.Core.Domain.Marketing;
using WinterSnow.Data;
using WinterSnow.Services.Payments;
using WinterSnow.Services.Payments.Providers;

namespace WinterSnow.Services.Orders;

public class CheckoutService : ICheckoutService
{
    private readonly WinterSnowDbContext _db;
    private readonly IAddressValidationService _addressValidation;
    private readonly IPaymentRoutingService _routing;
    private readonly IPaymentGatewayRegistry _gateways;

    public CheckoutService(
        WinterSnowDbContext db,
        IAddressValidationService addressValidation,
        IPaymentRoutingService routing,
        IPaymentGatewayRegistry gateways)
    {
        _db = db;
        _addressValidation = addressValidation;
        _routing = routing;
        _gateways = gateways;
    }

    public async Task<List<SplitOrderSummary>> PreviewSplitAsync(List<CheckoutItem> items, CancellationToken ct = default)
    {
        return await PreviewInternalAsync(items, couponCode: null, ct);
    }

    public async Task<List<SplitOrderSummary>> PreviewSplitV2Async(CheckoutPreviewRequestV2 request, CancellationToken ct = default)
    {
        return await PreviewInternalAsync(request.Items, request.CouponCode, ct);
    }

    private async Task<List<SplitOrderSummary>> PreviewInternalAsync(List<CheckoutItem> items, string? couponCode, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
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

            decimal basePrice = product.Price;
            if (item.VariantId is not null && variants.TryGetValue(item.VariantId.Value, out var variant) && variant.OverridePrice is not null)
                basePrice = variant.OverridePrice.Value;

            var discountActive =
                product.DiscountPercent is not null &&
                (product.DiscountStartUtc is null || product.DiscountStartUtc <= now) &&
                (product.DiscountEndUtc is null || product.DiscountEndUtc >= now) &&
                product.DiscountPercent.Value > 0;

            var unitPrice = discountActive
                ? Math.Round(basePrice * (1 - (product.DiscountPercent!.Value / 100m)), 2)
                : basePrice;

            if (!result.TryGetValue(product.VendorId, out var summary))
            {
                summary = new SplitOrderSummary
                {
                    VendorId = product.VendorId,
                    Currency = product.Currency
                };
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
            s.DiscountTotal = 0;
            s.OrderTotalAfterDiscount = s.OrderTotal;
        }

        // Coupon application (global fixed amount, allocated to eligible items)
        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var code = couponCode.Trim().ToUpperInvariant();
            var coupon = await _db.Coupons.FirstOrDefaultAsync(c =>
                c.Code == code &&
                c.IsActive &&
                (c.StartUtc == null || c.StartUtc <= now) &&
                (c.EndUtc == null || c.EndUtc >= now), ct);

            if (coupon is not null && coupon.DiscountAmount > 0)
            {
                // Eligible subtotal = items whose Product.AllowCoupons is true
                var eligibleByVendor = new Dictionary<int, decimal>();
                foreach (var s in result.Values)
                {
                    var eligible = 0m;
                    foreach (var line in s.Items)
                    {
                        if (products.TryGetValue(line.ProductId, out var p) && p.AllowCoupons)
                            eligible += line.UnitPrice * line.Quantity;
                    }
                    eligibleByVendor[s.VendorId] = eligible;
                }

                var eligibleTotal = eligibleByVendor.Values.Sum();
                if (eligibleTotal > 0)
                {
                    var maxDiscount = Math.Min(coupon.DiscountAmount, eligibleTotal);
                    foreach (var s in result.Values)
                    {
                        var eligible = eligibleByVendor[s.VendorId];
                        if (eligible <= 0)
                            continue;

                        var share = eligible / eligibleTotal;
                        var discount = Math.Round(maxDiscount * share, 2);
                        discount = Math.Min(discount, s.Subtotal); // don't exceed items subtotal

                        s.DiscountTotal = discount;
                        s.OrderTotalAfterDiscount = Math.Max(0, s.OrderTotal - discount);
                    }
                }
            }
        }

        return result.Values.OrderBy(x => x.VendorId).ToList();
    }

    public async Task<CheckoutResult> CreateOrdersAndPaymentSessionAsync(int customerId, CheckoutRequest request, CancellationToken ct = default)
    {
        var addressValidation = await _addressValidation.ValidateAsync(request.ShippingAddress, ct);
        if (!addressValidation.IsValid || addressValidation.Normalized is null)
            throw new InvalidOperationException(addressValidation.Message ?? "Invalid address.");

        var split = await PreviewInternalAsync(request.Items, request.CouponCode, ct);
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
        var paymentGroupId = Guid.NewGuid();

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
            var provider = await _routing.GetProviderForVendorAsync(vendorSplit.VendorId, ct);

            var order = new Order
            {
                CustomerId = customerId,
                VendorId = vendorSplit.VendorId,
                ShippingAddressId = address.Id,
                Subtotal = vendorSplit.Subtotal - vendorSplit.DiscountTotal,
                DiscountTotal = vendorSplit.DiscountTotal,
                ShippingFee = vendorSplit.OrderTotal - vendorSplit.Subtotal,
                TaxTotal = 0,
                OrderTotal = vendorSplit.OrderTotalAfterDiscount,
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
                PaymentGroupId = paymentGroupId,
                VendorId = order.VendorId,
                PaymentProviderId = provider.Id,
                PaymentProviderSystemName = provider.SystemName,
                PaymentProviderDisplayName = provider.DisplayName,
                Status = PaymentStatus.Pending,
                Amount = order.OrderTotal,
                Currency = order.Currency
            });
        }

        await _db.SaveChangesAsync(ct);

        var txns = await _db.PaymentTransactions
            .Where(p => p.PaymentGroupId == paymentGroupId)
            .ToListAsync(ct);

        var sessions = new List<PaymentSessionInfo>();
        foreach (var grp in txns.GroupBy(t => t.PaymentProviderSystemName, StringComparer.OrdinalIgnoreCase))
        {
            var systemName = grp.Key;
            var gateway = _gateways.GetRequired(systemName);
            var amount = grp.Sum(x => x.Amount);
            var currency = grp.First().Currency;

            var session = await gateway.CreatePaymentSessionAsync(new PaymentSessionRequest
            {
                CustomerEmail = customer.Email,
                Amount = amount,
                Currency = currency
            }, ct);

            foreach (var t in grp)
                t.ProviderPaymentSessionId = session.SessionId;

            sessions.Add(new PaymentSessionInfo
            {
                ProviderSystemName = session.ProviderSystemName,
                ProviderDisplayName = session.ProviderDisplayName,
                SessionId = session.SessionId,
                OrderIds = grp.Select(x => x.OrderId).ToList()
            });
        }

        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);

        var single = sessions.Count == 1 ? sessions[0].SessionId : null;
        return new CheckoutResult
        {
            CreatedOrderIds = createdOrderIds,
            SplitSummary = split,
            PaymentSessionId = single,
            PaymentSessions = sessions
        };
    }
}

