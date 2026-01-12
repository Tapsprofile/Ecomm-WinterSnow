using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Notifications;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationInboxService _inbox;

    public NotificationsController(INotificationInboxService inbox)
    {
        _inbox = inbox;
    }

    private int UserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : 0;
    private string? Role => User.FindFirstValue(ClaimTypes.Role);
    private int VendorId => int.TryParse(User.FindFirstValue("vendorId"), out var id) ? id : 0;

    [HttpGet("unread-count")]
    public async Task<ActionResult<object>> UnreadCount(CancellationToken ct)
    {
        if (Role == "Vendor")
            return Ok(new { unread = await _inbox.GetUnreadCountForVendorAsync(VendorId, ct) });
        if (Role == "Customer")
            return Ok(new { unread = await _inbox.GetUnreadCountForCustomerAsync(UserId, ct) });
        return Ok(new { unread = 0 });
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> Inbox([FromQuery] int take = 20, CancellationToken ct = default)
    {
        if (Role == "Vendor")
            return Ok(await _inbox.GetInboxForVendorAsync(VendorId, take, ct));
        if (Role == "Customer")
            return Ok(await _inbox.GetInboxForCustomerAsync(UserId, take, ct));
        return Ok(new List<NotificationDto>());
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkRead([FromRoute] int id, CancellationToken ct)
    {
        await _inbox.MarkReadAsync(id, Role == "Customer" ? UserId : null, Role == "Vendor" ? VendorId : null, ct);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await _inbox.MarkAllReadAsync(Role == "Customer" ? UserId : null, Role == "Vendor" ? VendorId : null, ct);
        return NoContent();
    }
}

