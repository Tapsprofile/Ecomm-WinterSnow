using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Data;
using WinterSnow.Services.Notifications;
using WinterSnow.Services.Vendor;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor")]
public class VendorController : ControllerBase
{
    private readonly IVendorDashboardService _dashboard;
    private readonly IVendorListingService _listings;
    private readonly INotificationQueue _notifications;
    private readonly WinterSnowDbContext _db;

    public VendorController(IVendorDashboardService dashboard, IVendorListingService listings, INotificationQueue notifications, WinterSnowDbContext db)
    {
        _dashboard = dashboard;
        _listings = listings;
        _notifications = notifications;
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

    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryDto>>> Categories(CancellationToken ct)
        => Ok(await _listings.GetCategoriesAsync(ct));

    [HttpPost("listings")]
    public async Task<ActionResult<object>> CreateListing([FromBody] CreateListingRequest req, CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();

        var id = await _listings.CreateListingAsync(VendorId, req, ct);

        await _notifications.EnqueueAsync(new NotificationMessage
        {
            RecipientType = WinterSnow.Core.Domain.Notifications.NotificationRecipientType.Vendor,
            RecipientVendorId = VendorId,
            Title = "Listing submitted",
            Body = $"Your listing \"{req.Name}\" was submitted for admin approval.",
            ActionUrl = "/vendor/inventory"
        }, ct);

        return Ok(new { productId = id });
    }

    [HttpPut("listings/{productId:int}")]
    public async Task<IActionResult> UpdateListing([FromRoute] int productId, [FromBody] UpdateListingRequest req, CancellationToken ct)
    {
        if (VendorId <= 0)
            return Unauthorized();
        await _listings.UpdateListingAsync(VendorId, productId, req, ct);
        return NoContent();
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

