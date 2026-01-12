using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Reviews;
using WinterSnow.Data.Repositories;

namespace WinterSnow.Services.Discovery;

public class SearchService : ISearchService
{
    private readonly IRepository<Product> _products;
    private readonly IRepository<ProductVariant> _variants;
    private readonly IRepository<ProductMedia> _media;
    private readonly IRepository<Review> _reviews;
    private readonly IRepository<ProductPostcodeVisibility> _postcodes;

    public SearchService(
        IRepository<Product> products,
        IRepository<ProductVariant> variants,
        IRepository<ProductMedia> media,
        IRepository<Review> reviews,
        IRepository<ProductPostcodeVisibility> postcodes)
    {
        _products = products;
        _variants = variants;
        _media = media;
        _reviews = reviews;
        _postcodes = postcodes;
    }

    public async Task<List<string>> AutocompleteAsync(string q, CancellationToken ct = default)
    {
        var term = q.Trim();
        if (term.Length < 2)
            return [];

        return await _products.Table
            .Where(p => p.Published && p.IsApprovedByAdmin)
            .Where(p => p.Name.Contains(term))
            .OrderBy(p => p.Name)
            .Select(p => p.Name)
            .Distinct()
            .Take(8)
            .ToListAsync(ct);
    }

    public async Task<SearchResponse> SearchAsync(SearchQuery query, CancellationToken ct = default)
    {
        var q = query.Q?.Trim();

        var now = DateTime.UtcNow;

        // Only show customer-visible listings:
        // - approved + published
        // - Active listing status
        // - vendor marked visible in storefront
        // - at least one variant has stock > 0
        var inStockProductIds = _variants.Table
            .Where(v => v.StockQuantity > 0)
            .Select(v => v.ProductId)
            .Distinct();

        var baseProducts = _products.Table
            .Where(p => p.Published && p.IsApprovedByAdmin)
            .Where(p => p.IsVisibleInStorefront)
            .Where(p => p.ListingStatus == WinterSnow.Core.Domain.Catalog.Listings.ListingStatus.Active)
            .Where(p => inStockProductIds.Contains(p.Id));

        // Postcode visibility:
        // - If a product has any entries in ProductPostcodeVisibility, it's only shown for matching postal codes.
        // - If no postal code is provided by the shopper, restricted products are hidden.
        var restrictedProductIds = _postcodes.Table.Select(x => x.ProductId).Distinct();
        if (string.IsNullOrWhiteSpace(query.PostalCode))
        {
            baseProducts = baseProducts.Where(p => !restrictedProductIds.Contains(p.Id));
        }
        else
        {
            var pc = query.PostalCode.Trim();
            var allowedProductIds = _postcodes.Table.Where(x => x.PostalCode == pc).Select(x => x.ProductId).Distinct();
            baseProducts = baseProducts.Where(p => !restrictedProductIds.Contains(p.Id) || allowedProductIds.Contains(p.Id));
        }

        if (!string.IsNullOrWhiteSpace(q))
            baseProducts = baseProducts.Where(p => p.Name.Contains(q) || (p.ShortDescription != null && p.ShortDescription.Contains(q)));

        if (!string.IsNullOrWhiteSpace(query.Material))
            baseProducts = baseProducts.Where(p => p.Material == query.Material);

        if (query.MinPrice is not null)
            baseProducts = baseProducts.Where(p => p.Price >= query.MinPrice);

        if (query.MaxPrice is not null)
            baseProducts = baseProducts.Where(p => p.Price <= query.MaxPrice);

        // Variant facet filters (size/color)
        if (!string.IsNullOrWhiteSpace(query.Size) || !string.IsNullOrWhiteSpace(query.Color))
        {
            var variantQuery = _variants.Table.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Size))
                variantQuery = variantQuery.Where(v => v.Size == query.Size);
            if (!string.IsNullOrWhiteSpace(query.Color))
                variantQuery = variantQuery.Where(v => v.Color == query.Color);

            var productIds = variantQuery.Select(v => v.ProductId).Distinct();
            baseProducts = baseProducts.Where(p => productIds.Contains(p.Id));
        }

        var total = await baseProducts.CountAsync(ct);

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var productPage = await baseProducts
            .OrderByDescending(p => p.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Slug,
                p.Price,
                p.Currency,
                p.AllowCoupons,
                p.DiscountPercent,
                p.DiscountStartUtc,
                p.DiscountEndUtc
            })
            .ToListAsync(ct);

        var productIdsPage = productPage.Select(x => x.Id).ToList();

        var thumbs = await _media.Table
            .Where(m => productIdsPage.Contains(m.ProductId))
            .OrderBy(m => m.DisplayOrder)
            .GroupBy(m => m.ProductId)
            .Select(g => new { ProductId = g.Key, Url = g.Select(x => x.Url).FirstOrDefault() })
            .ToListAsync(ct);

        var ratings = await _reviews.Table
            .Where(r => productIdsPage.Contains(r.ProductId))
            .GroupBy(r => r.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Avg = g.Average(x => (double)x.Rating),
                Count = g.Count()
            })
            .ToListAsync(ct);

        // Facets are computed over current baseProducts (pre-pagination) for UX.
        var facetProductIds = baseProducts.Select(p => p.Id);

        var sizeBuckets = await _variants.Table
            .Where(v => facetProductIds.Contains(v.ProductId) && v.Size != null)
            .GroupBy(v => v.Size!)
            .Select(g => new FacetBucket { Value = g.Key, Count = g.Select(x => x.ProductId).Distinct().Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Value)
            .Take(20)
            .ToListAsync(ct);

        var colorBuckets = await _variants.Table
            .Where(v => facetProductIds.Contains(v.ProductId) && v.Color != null)
            .GroupBy(v => v.Color!)
            .Select(g => new FacetBucket { Value = g.Key, Count = g.Select(x => x.ProductId).Distinct().Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Value)
            .Take(20)
            .ToListAsync(ct);

        var materialBuckets = await baseProducts
            .Where(p => p.Material != null)
            .GroupBy(p => p.Material!)
            .Select(g => new FacetBucket { Value = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Value)
            .Take(20)
            .ToListAsync(ct);

        return new SearchResponse
        {
            Total = total,
            Items = productPage.Select(p =>
            {
                var thumb = thumbs.FirstOrDefault(t => t.ProductId == p.Id)?.Url;
                var rating = ratings.FirstOrDefault(r => r.ProductId == p.Id);

                var discountActive =
                    p.DiscountPercent is not null &&
                    (p.DiscountStartUtc is null || p.DiscountStartUtc <= now) &&
                    (p.DiscountEndUtc is null || p.DiscountEndUtc >= now) &&
                    p.DiscountPercent.Value > 0;

                var finalPrice = discountActive
                    ? Math.Round(p.Price * (1 - (p.DiscountPercent!.Value / 100m)), 2)
                    : p.Price;

                return new SearchResultItem
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Price = finalPrice,
                    OriginalPrice = discountActive ? p.Price : null,
                    DiscountPercent = discountActive ? p.DiscountPercent : null,
                    AllowCoupons = p.AllowCoupons,
                    Currency = p.Currency,
                    ThumbnailUrl = thumb,
                    RatingAvg = rating?.Avg ?? 0,
                    RatingCount = rating?.Count ?? 0
                };
            }).ToList(),
            Facets = new SearchFacets
            {
                Sizes = sizeBuckets,
                Colors = colorBuckets,
                Materials = materialBuckets
            }
        };
    }
}

