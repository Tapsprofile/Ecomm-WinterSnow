using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Data;
using WinterSnow.Services.Vendor;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor")]
public class VendorController : ControllerBase
{
    private readonly IVendorDashboardService _dashboard;
    private readonly WinterSnowDbContext _db;

    public VendorController(IVendorDashboardService dashboard, WinterSnowDbContext db)
    {
        _dashboard = dashboard;
        _db = db;
    }

    private int VendorId => int.TryParse(User.FindFirstValue("vendorId"), out var id) ? id : 0;

    [HttpGet("dashboard/overview")]
    public async Task<ActionResult<VendorSalesOverview>> Overview(CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();
        return Ok(await _dashboard.GetSalesOverviewAsync(VendorId, ct));
    }

    [HttpGet("inventory")]
    public async Task<ActionResult<List<VendorInventoryItem>>> Inventory(CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();
        return Ok(await _dashboard.GetInventoryAsync(VendorId, ct));
    }

    [HttpPost("inventory/bulk-update")]
    public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateInventoryRequest req, CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();
        await _dashboard.BulkUpdateInventoryAsync(VendorId, req, ct);
        return NoContent();
    }

    [HttpGet("orders")]
    public async Task<ActionResult<object>> Orders([FromQuery] OrderStatus? status, CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();

        var q = _db.Orders.Where(o => o.VendorId == VendorId);
        if (status is not null)
            q = q.Where(o => o.Status == status);

        var orders = await q
            .OrderByDescending(o => o.CreatedOnUtc)
            .Take(50)
            .Select(o => new
            {
                orderId = o.Id,
                status = o.Status.ToString(),
                total = o.OrderTotal,
                currency = o.Currency,
                createdOnUtc = o.CreatedOnUtc
            })
            .ToListAsync(ct);

        return Ok(new { orders });
    }

    [HttpGet("payouts")]
    public async Task<ActionResult<object>> Payouts(CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();

        var totalSales = await _db.VendorLedgerEntries
            .Where(e => e.VendorId == VendorId)
            .SumAsync(e => (decimal?)e.Amount, ct) ?? 0;

        var nextPayoutDateUtc = DateTime.UtcNow.Date.AddDays(30);

        return Ok(new
        {
            totalSales,
            nextPayoutDateUtc
        });
    }
}

