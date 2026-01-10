using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Data;

namespace WinterSnow.Services.Vendor;

public class VendorDashboardService : IVendorDashboardService
{
    private readonly WinterSnowDbContext _db;

    public VendorDashboardService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<VendorSalesOverview> GetSalesOverviewAsync(int vendorId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-7);

        var orders = _db.Orders.Where(o => o.VendorId == vendorId && o.Status != OrderStatus.Cancelled);

        var revenueToday = await orders
            .Where(o => o.CreatedOnUtc >= today)
            .SumAsync(o => (decimal?)o.OrderTotal, ct) ?? 0;

        var revenueThisWeek = await orders
            .Where(o => o.CreatedOnUtc >= weekStart)
            .SumAsync(o => (decimal?)o.OrderTotal, ct) ?? 0;

        var completed = orders.Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Shipped);
        var completedCount = await completed.CountAsync(ct);
        var completedRevenue = await completed.SumAsync(o => (decimal?)o.OrderTotal, ct) ?? 0;

        var top = await _db.OrderItems
            .Where(oi => _db.Orders.Any(o => o.Id == oi.OrderId && o.VendorId == vendorId && o.CreatedOnUtc >= weekStart))
            .GroupBy(oi => new { oi.ProductId, oi.ProductName })
            .Select(g => new TopProduct
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.ProductName,
                UnitsSold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.PriceInclTax)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(5)
            .ToListAsync(ct);

        return new VendorSalesOverview
        {
            RevenueToday = revenueToday,
            RevenueThisWeek = revenueThisWeek,
            AverageOrderValue = completedCount == 0 ? 0 : Math.Round(completedRevenue / completedCount, 2),
            TopWinterProducts = top
        };
    }

    public async Task<List<VendorInventoryItem>> GetInventoryAsync(int vendorId, CancellationToken ct = default)
    {
        var products = await _db.Products
            .Where(p => p.VendorId == vendorId)
            .OrderByDescending(p => p.Id)
            .ToListAsync(ct);

        var productIds = products.Select(p => p.Id).ToList();
        var variants = await _db.ProductVariants
            .Where(v => productIds.Contains(v.ProductId))
            .OrderBy(v => v.ProductId)
            .ThenBy(v => v.Id)
            .ToListAsync(ct);

        return products.Select(p => new VendorInventoryItem
        {
            ProductId = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Price = p.Price,
            Published = p.Published,
            IsApprovedByAdmin = p.IsApprovedByAdmin,
            Variants = variants.Where(v => v.ProductId == p.Id).Select(v => new InventoryVariant
            {
                VariantId = v.Id,
                Size = v.Size,
                Color = v.Color,
                OverridePrice = v.OverridePrice,
                StockQuantity = v.StockQuantity
            }).ToList()
        }).ToList();
    }

    public async Task BulkUpdateInventoryAsync(int vendorId, BulkUpdateInventoryRequest request, CancellationToken ct = default)
    {
        var variantIds = request.Variants.Select(v => v.VariantId).Distinct().ToList();
        if (variantIds.Count == 0)
            return;

        var variants = await _db.ProductVariants.Where(v => variantIds.Contains(v.Id)).ToListAsync(ct);

        // Security: ensure these variants belong to this vendor
        var productIds = variants.Select(v => v.ProductId).Distinct().ToList();
        var vendorProductIds = await _db.Products.Where(p => p.VendorId == vendorId && productIds.Contains(p.Id)).Select(p => p.Id).ToListAsync(ct);

        foreach (var patch in request.Variants)
        {
            var v = variants.FirstOrDefault(x => x.Id == patch.VariantId);
            if (v is null || !vendorProductIds.Contains(v.ProductId))
                continue;

            if (patch.OverridePrice is not null)
                v.OverridePrice = patch.OverridePrice;

            if (patch.StockQuantity is not null)
                v.StockQuantity = Math.Max(0, patch.StockQuantity.Value);
        }

        await _db.SaveChangesAsync(ct);
    }
}

