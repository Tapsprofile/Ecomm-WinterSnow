using Microsoft.EntityFrameworkCore;
using WinterSnow.Data;

namespace WinterSnow.Services.Admin;

public class AdminService : IAdminService
{
    private readonly WinterSnowDbContext _db;

    public AdminService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public Task<List<PendingVendor>> GetPendingVendorsAsync(CancellationToken ct = default)
        => _db.Vendors
            .Where(v => !v.IsKycApproved)
            .OrderBy(v => v.Id)
            .Select(v => new PendingVendor
            {
                VendorId = v.Id,
                Name = v.Name,
                IsKycApproved = v.IsKycApproved,
                IsActive = v.IsActive
            })
            .ToListAsync(ct);

    public Task<List<PendingProduct>> GetPendingProductsAsync(CancellationToken ct = default)
        => _db.Products
            .Where(p => !p.IsApprovedByAdmin)
            .OrderBy(p => p.Id)
            .Select(p => new PendingProduct
            {
                ProductId = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                VendorId = p.VendorId,
                Published = p.Published,
                IsApprovedByAdmin = p.IsApprovedByAdmin
            })
            .ToListAsync(ct);

    public async Task ApproveVendorKycAsync(int vendorId, CancellationToken ct = default)
    {
        var vendor = await _db.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId, ct);
        if (vendor is null)
            return;
        vendor.IsKycApproved = true;
        vendor.IsActive = true;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ApproveProductAsync(int productId, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
        if (product is null)
            return;
        product.IsApprovedByAdmin = true;
        product.Published = true;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<FinanceSummary> GetFinanceSummaryAsync(CancellationToken ct = default)
    {
        var ordersCount = await _db.Orders.CountAsync(ct);
        var revenueGross = await _db.Orders.SumAsync(o => (decimal?)o.OrderTotal, ct) ?? 0;

        var commissions = await _db.VendorLedgerEntries
            .Where(e => e.Description == "Platform commission")
            .SumAsync(e => (decimal?)e.Amount, ct) ?? 0;

        // commission entries are negative
        var totalCommissionsEarned = Math.Abs(commissions);

        return new FinanceSummary
        {
            OrdersCount = ordersCount,
            PlatformRevenueGross = revenueGross,
            TotalCommissionsEarned = totalCommissionsEarned
        };
    }
}

