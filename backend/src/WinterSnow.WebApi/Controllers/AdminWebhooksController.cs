using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Webhooks;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/webhooks")]
public class AdminWebhooksController : ControllerBase
{
    private readonly IWebhookAdminService _webhooks;

    public AdminWebhooksController(IWebhookAdminService webhooks)
    {
        _webhooks = webhooks;
    }

    [HttpGet]
    public Task<List<WebhookSubscriptionDto>> List(CancellationToken ct)
        => _webhooks.ListAsync(ct);

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateWebhookSubscriptionRequest req, CancellationToken ct)
    {
        var id = await _webhooks.CreateAsync(req, ct);
        return Ok(new { id });
    }

    public class SetActiveRequest
    {
        public bool IsActive { get; set; }
    }

    [HttpPost("{id:int}/active")]
    public async Task<IActionResult> SetActive([FromRoute] int id, [FromBody] SetActiveRequest req, CancellationToken ct)
    {
        await _webhooks.SetActiveAsync(id, req.IsActive, ct);
        return NoContent();
    }
}

