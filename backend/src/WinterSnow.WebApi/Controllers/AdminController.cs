using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Marketing;
using WinterSnow.Data;
using WinterSnow.Services.Admin;
using WinterSnow.Services.System;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;
    private readonly WinterSnowDbContext _db;
    private readonly ApiMetrics _metrics;

    public AdminController(IAdminService admin, WinterSnowDbContext db, ApiMetrics metrics)
    {
        _admin = admin;
        _db = db;
        _metrics = metrics;
    }

    [HttpGet("moderation/vendors")]
    public Task<List<PendingVendor>> PendingVendors(CancellationToken ct)
        => _admin.GetPendingVendorsAsync(ct);

    [HttpPost("moderation/vendors/{vendorId:int}/approve-kyc")]
    public async Task<IActionResult> ApproveVendorKyc([FromRoute] int vendorId, CancellationToken ct)
    {
        await _admin.ApproveVendorKycAsync(vendorId, ct);
        return NoContent();
    }

    [HttpGet("moderation/products")]
    public Task<List<PendingProduct>> PendingProducts(CancellationToken ct)
        => _admin.GetPendingProductsAsync(ct);

    [HttpPost("moderation/products/{productId:int}/approve")]
    public async Task<IActionResult> ApproveProduct([FromRoute] int productId, CancellationToken ct)
    {
        await _admin.ApproveProductAsync(productId, ct);
        return NoContent();
    }

    [HttpGet("finance/summary")]
    public Task<FinanceSummary> FinanceSummary(CancellationToken ct)
        => _admin.GetFinanceSummaryAsync(ct);

    [HttpGet("system/health")]
    public ActionResult<object> SystemHealth()
    {
        var (p50, p95) = _metrics.GetPercentiles();
        return Ok(new
        {
            status = "OK",
            uptimeSeconds = (DateTime.UtcNow - _metrics.StartedUtc).TotalSeconds,
            apiResponseP50Ms = p50,
            apiResponseP95Ms = p95,
            recentErrors = _metrics.GetRecentErrors()
        });
    }

    // Marketing Tools (starter CRUD)
    [HttpGet("marketing/banners")]
    public Task<List<Banner>> Banners(CancellationToken ct)
        => _db.Banners.OrderByDescending(b => b.Id).Take(50).ToListAsync(ct);

    [HttpPost("marketing/banners")]
    public async Task<ActionResult<Banner>> CreateBanner([FromBody] Banner banner, CancellationToken ct)
    {
        _db.Banners.Add(banner);
        await _db.SaveChangesAsync(ct);
        return Ok(banner);
    }

    [HttpGet("marketing/coupons")]
    public Task<List<Coupon>> Coupons(CancellationToken ct)
        => _db.Coupons.OrderByDescending(c => c.Id).Take(50).ToListAsync(ct);

    [HttpPost("marketing/coupons")]
    public async Task<ActionResult<Coupon>> CreateCoupon([FromBody] Coupon coupon, CancellationToken ct)
    {
        coupon.Code = coupon.Code.Trim().ToUpperInvariant();
        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync(ct);
        return Ok(coupon);
    }

    [HttpGet("marketing/flash-sales")]
    public Task<List<FlashSaleEvent>> FlashSales(CancellationToken ct)
        => _db.FlashSaleEvents.OrderByDescending(f => f.Id).Take(50).ToListAsync(ct);

    [HttpPost("marketing/flash-sales")]
    public async Task<ActionResult<FlashSaleEvent>> CreateFlashSale([FromBody] FlashSaleEvent ev, CancellationToken ct)
    {
        _db.FlashSaleEvents.Add(ev);
        await _db.SaveChangesAsync(ct);
        return Ok(ev);
    }
}

