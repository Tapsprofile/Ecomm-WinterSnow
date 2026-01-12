using Microsoft.EntityFrameworkCore;
using WinterSnow.Data;

namespace WinterSnow.Services.Customers;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly WinterSnowDbContext _db;

    public CustomerOrderService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<List<CustomerOrderListItem>> GetMyOrdersAsync(int customerId, CancellationToken ct = default)
    {
        var orders = await _db.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedOnUtc)
            .Take(100)
            .Select(o => new CustomerOrderListItem
            {
                OrderId = o.Id,
                VendorId = o.VendorId,
                Status = o.Status.ToString(),
                OrderTotal = o.OrderTotal,
                Currency = o.Currency,
                CreatedOnUtc = o.CreatedOnUtc,
                ItemsCount = 0
            })
            .ToListAsync(ct);

        if (orders.Count == 0)
            return orders;

        var orderIds = orders.Select(o => o.OrderId).ToList();
        var counts = await _db.OrderItems
            .Where(i => orderIds.Contains(i.OrderId))
            .GroupBy(i => i.OrderId)
            .Select(g => new { OrderId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OrderId, x => x.Count, ct);

        foreach (var o in orders)
            o.ItemsCount = counts.TryGetValue(o.OrderId, out var c) ? c : 0;

        return orders;
    }

    public async Task<CustomerOrderDetailsDto?> GetMyOrderAsync(int customerId, int orderId, CancellationToken ct = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId, ct);
        if (order is null)
            return null;

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == order.ShippingAddressId, ct);

        var items = await _db.OrderItems
            .Where(i => i.OrderId == order.Id)
            .OrderBy(i => i.Id)
            .Select(i => new CustomerOrderItemDto
            {
                OrderItemId = i.Id,
                ProductId = i.ProductId,
                ProductVariantId = i.ProductVariantId,
                ProductName = i.ProductName,
                UnitPriceInclTax = i.UnitPriceInclTax,
                Quantity = i.Quantity,
                PriceInclTax = i.PriceInclTax
            })
            .ToListAsync(ct);

        return new CustomerOrderDetailsDto
        {
            OrderId = order.Id,
            VendorId = order.VendorId,
            Status = order.Status.ToString(),
            Subtotal = order.Subtotal,
            DiscountTotal = order.DiscountTotal,
            ShippingFee = order.ShippingFee,
            TaxTotal = order.TaxTotal,
            OrderTotal = order.OrderTotal,
            Currency = order.Currency,
            CreatedOnUtc = order.CreatedOnUtc,
            ShippingAddress = address is null
                ? new CustomerOrderAddressDto()
                : new CustomerOrderAddressDto
                {
                    FullName = address.FullName,
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    CountryCode = address.CountryCode,
                    Phone = address.Phone
                },
            Items = items
        };
    }
}

