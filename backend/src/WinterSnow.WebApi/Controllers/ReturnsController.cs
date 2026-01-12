using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Returns;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Route("api/returns")]
public class ReturnsController : ControllerBase
{
    private readonly IReturnService _returns;

    public ReturnsController(IReturnService returns)
    {
        _returns = returns;
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("me")]
    public async Task<ActionResult<List<ReturnDto>>> MyReturns(CancellationToken ct)
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        return Ok(await _returns.GetCustomerReturnsAsync(customerId, ct));
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateReturnRequest req, CancellationToken ct)
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        var id = await _returns.CreateAsync(customerId, req, ct);
        return Ok(new { returnId = id });
    }

    [Authorize(Roles = "Vendor")]
    [HttpGet("vendor")]
    public async Task<ActionResult<List<ReturnDto>>> VendorReturns(CancellationToken ct)
    {
        var vendorId = int.TryParse(User.FindFirstValue("vendorId"), out var id) ? id : 0;
        if (vendorId <= 0) return Unauthorized();
        return Ok(await _returns.GetVendorReturnsAsync(vendorId, ct));
    }

    public class VendorDecisionRequest
    {
        public bool Approve { get; set; }
    }

    [Authorize(Roles = "Vendor")]
    [HttpPost("{returnId:int}/vendor-decision")]
    public async Task<IActionResult> VendorDecision([FromRoute] int returnId, [FromBody] VendorDecisionRequest req, CancellationToken ct)
    {
        var vendorId = int.TryParse(User.FindFirstValue("vendorId"), out var id) ? id : 0;
        if (vendorId <= 0) return Unauthorized();
        await _returns.VendorApproveAsync(vendorId, returnId, req.Approve, ct);
        return NoContent();
    }
}

