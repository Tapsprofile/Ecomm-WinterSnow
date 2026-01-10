using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Catalog;
using WinterSnow.Services.Discovery;
using WinterSnow.Services.Orders;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Route("api/storefront")]
public class StorefrontController : ControllerBase
{
    private readonly IProductCatalogService _catalog;
    private readonly ISearchService _search;
    private readonly IAddressValidationService _addressValidation;
    private readonly ICheckoutService _checkout;

    public StorefrontController(
        IProductCatalogService catalog,
        ISearchService search,
        IAddressValidationService addressValidation,
        ICheckoutService checkout)
    {
        _catalog = catalog;
        _search = search;
        _addressValidation = addressValidation;
        _checkout = checkout;
    }

    [HttpGet("home")]
    public async Task<ActionResult<object>> Home(CancellationToken ct)
    {
        var topPicks = await _catalog.GetHomepageSectionAsync("top-picks", ct);
        var newArrivals = await _catalog.GetHomepageSectionAsync("new-arrivals", ct);
        var winterCollections = await _catalog.GetHomepageSectionAsync("winter-collections", ct);

        return Ok(new
        {
            topPicks,
            newArrivals,
            winterCollections
        });
    }

    [HttpGet("search/autocomplete")]
    public async Task<ActionResult<List<string>>> Autocomplete([FromQuery] string q, CancellationToken ct)
        => Ok(await _search.AutocompleteAsync(q, ct));

    [HttpGet("search")]
    public async Task<ActionResult<SearchResponse>> Search([FromQuery] SearchQuery query, CancellationToken ct)
        => Ok(await _search.SearchAsync(query, ct));

    [HttpGet("products/{slug}")]
    public async Task<ActionResult<ProductDetails>> ProductBySlug([FromRoute] string slug, CancellationToken ct)
    {
        var product = await _catalog.GetProductBySlugAsync(slug, ct);
        if (product is null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost("checkout/validate-address")]
    public Task<AddressValidationResult> ValidateAddress([FromBody] ShippingAddressInput input, CancellationToken ct)
        => _addressValidation.ValidateAsync(input, ct);

    [HttpPost("checkout/preview")]
    public async Task<ActionResult<List<SplitOrderSummary>>> Preview([FromBody] List<CheckoutItem> items, CancellationToken ct)
        => Ok(await _checkout.PreviewSplitAsync(items, ct));

    [Authorize(Roles = "Customer")]
    [HttpPost("checkout/submit")]
    public async Task<ActionResult<CheckoutResult>> Submit([FromBody] CheckoutRequest req, CancellationToken ct)
    {
        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0");
        if (customerId <= 0)
            return Unauthorized();

        return Ok(await _checkout.CreateOrdersAndPaymentSessionAsync(customerId, req, ct));
    }
}

