using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Payments;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor/payments")]
public class VendorPaymentsController : ControllerBase
{
    private readonly IPaymentAdminService _payments;

    public VendorPaymentsController(IPaymentAdminService payments)
    {
        _payments = payments;
    }

    private int VendorId => int.TryParse(User.FindFirstValue("vendorId"), out var id) ? id : 0;

    [HttpGet("transactions")]
    public Task<List<PaymentTransactionDto>> Transactions([FromQuery] PaymentTransactionQuery query, CancellationToken ct)
    {
        // Force vendor scoping
        query.VendorId = VendorId;
        return _payments.QueryTransactionsAsync(query, ct);
    }
}

