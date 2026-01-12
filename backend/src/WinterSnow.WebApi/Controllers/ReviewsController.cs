using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Reviews;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviews;

    public ReviewsController(IReviewService reviews)
    {
        _reviews = reviews;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<ActionResult<CreateReviewResult>> Create([FromBody] CreateReviewRequest req, CancellationToken ct)
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        if (customerId <= 0) return Unauthorized();

        return Ok(await _reviews.CreateAsync(customerId, req, ct));
    }
}

