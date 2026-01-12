using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Customers;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly ICustomerOrderService _orders;

    public AccountController(ICustomerOrderService orders)
    {
        _orders = orders;
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("me/orders")]
    public async Task<ActionResult<List<CustomerOrderListItem>>> MyOrders(CancellationToken ct)
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        if (customerId <= 0) return Unauthorized();

        return Ok(await _orders.GetMyOrdersAsync(customerId, ct));
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("me/orders/{orderId:int}")]
    public async Task<ActionResult<CustomerOrderDetailsDto>> MyOrder([FromRoute] int orderId, CancellationToken ct)
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        if (customerId <= 0) return Unauthorized();

        var order = await _orders.GetMyOrderAsync(customerId, orderId, ct);
        if (order is null) return NotFound();
        return Ok(order);
    }
}

