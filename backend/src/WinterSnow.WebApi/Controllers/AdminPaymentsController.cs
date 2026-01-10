using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Core.Domain.Payments.Providers;
using WinterSnow.Services.Payments;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/payments")]
public class AdminPaymentsController : ControllerBase
{
    private readonly IPaymentAdminService _payments;

    public AdminPaymentsController(IPaymentAdminService payments)
    {
        _payments = payments;
    }

    // Providers
    [HttpGet("providers")]
    public Task<List<PaymentProvider>> Providers(CancellationToken ct)
        => _payments.ListProvidersAsync(ct);

    public class CreateProviderRequest
    {
        public required string SystemName { get; set; }
        public required string DisplayName { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ConfigJson { get; set; }
    }

    [HttpPost("providers")]
    public async Task<ActionResult<object>> CreateProvider([FromBody] CreateProviderRequest req, CancellationToken ct)
    {
        var id = await _payments.CreateProviderAsync(req.SystemName, req.DisplayName, req.IsActive, req.ConfigJson, ct);
        return Ok(new { providerId = id });
    }

    public class SetActiveRequest
    {
        public bool IsActive { get; set; }
    }

    [HttpPost("providers/{providerId:int}/active")]
    public async Task<IActionResult> SetProviderActive([FromRoute] int providerId, [FromBody] SetActiveRequest req, CancellationToken ct)
    {
        await _payments.SetProviderActiveAsync(providerId, req.IsActive, ct);
        return NoContent();
    }

    // Vendor mapping
    [HttpGet("vendors/{vendorId:int}/providers")]
    public Task<List<VendorPaymentProvider>> VendorProviders([FromRoute] int vendorId, CancellationToken ct)
        => _payments.ListVendorProvidersAsync(vendorId, ct);

    public class SetVendorProviderRequest
    {
        public int ProviderId { get; set; }
        public bool IsActive { get; set; } = true;
        public int Priority { get; set; } = 0;
    }

    [HttpPost("vendors/{vendorId:int}/providers")]
    public async Task<IActionResult> SetVendorProvider([FromRoute] int vendorId, [FromBody] SetVendorProviderRequest req, CancellationToken ct)
    {
        await _payments.SetVendorProviderAsync(vendorId, req.ProviderId, req.IsActive, req.Priority, ct);
        return NoContent();
    }

    // Transaction search
    [HttpGet("transactions")]
    public Task<List<PaymentTransactionDto>> Transactions([FromQuery] PaymentTransactionQuery query, CancellationToken ct)
        => _payments.QueryTransactionsAsync(query, ct);
}

