using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Reviews;
using WinterSnow.Data.Repositories;

namespace WinterSnow.Services.Catalog;

public class ProductCatalogService : IProductCatalogService
{
    private readonly IRepository<Product> _products;
    private readonly IRepository<ProductVariant> _variants;
    private readonly IRepository<ProductMedia> _media;
    private readonly IRepository<Review> _reviews;
    private readonly IRepository<ReviewMedia> _reviewMedia;

    public ProductCatalogService(
        IRepository<Product> products,
        IRepository<ProductVariant> variants,
        IRepository<ProductMedia> media,
        IRepository<Review> reviews,
        IRepository<ReviewMedia> reviewMedia)
    {
        _products = products;
        _variants = variants;
        _media = media;
        _reviews = reviews;
        _reviewMedia = reviewMedia;
    }

    public async Task<ProductDetails?> GetProductBySlugAsync(string slug, CancellationToken ct = default)
    {
        var s = slug.Trim().ToLowerInvariant();
        var product = await _products.Table
            .Where(p => p.Published && p.IsApprovedByAdmin)
            .FirstOrDefaultAsync(p => p.Slug.ToLower() == s, ct);

        if (product is null)
            return null;

        var media = await _media.Table
            .Where(m => m.ProductId == product.Id)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new ProductMediaDto { Url = m.Url, MediaType = m.MediaType, DisplayOrder = m.DisplayOrder })
            .ToListAsync(ct);

        var variants = await _variants.Table
            .Where(v => v.ProductId == product.Id)
            .OrderBy(v => v.Id)
            .Select(v => new ProductVariantDto
            {
                VariantId = v.Id,
                Size = v.Size,
                Color = v.Color,
                Sku = v.Sku,
                EffectivePrice = v.OverridePrice ?? product.Price,
                StockQuantity = v.StockQuantity
            })
            .ToListAsync(ct);

        var reviews = await _reviews.Table
            .Where(r => r.ProductId == product.Id)
            .OrderByDescending(r => r.CreatedOnUtc)
            .Take(20)
            .Select(r => new ReviewDto
            {
                ReviewId = r.Id,
                Rating = r.Rating,
                Title = r.Title,
                ReviewText = r.ReviewText,
                IsVerifiedPurchase = r.IsVerifiedPurchase,
                CreatedOnUtc = r.CreatedOnUtc
            })
            .ToListAsync(ct);

        var reviewIds = reviews.Select(r => r.ReviewId).ToList();
        var mediaByReview = await _reviewMedia.Table
            .Where(m => reviewIds.Contains(m.ReviewId))
            .GroupBy(m => m.ReviewId)
            .Select(g => new { ReviewId = g.Key, Urls = g.Select(x => x.Url).ToList() })
            .ToListAsync(ct);

        foreach (var r in reviews)
            r.MediaUrls = mediaByReview.FirstOrDefault(x => x.ReviewId == r.ReviewId)?.Urls ?? [];

        return new ProductDetails
        {
            ProductId = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            FullDescription = product.FullDescription,
            Material = product.Material,
            Price = product.Price,
            Currency = product.Currency,
            Media = media,
            Variants = variants,
            Reviews = reviews
        };
    }

    public async Task<List<ProductCard>> GetHomepageSectionAsync(string sectionKey, CancellationToken ct = default)
    {
        var key = sectionKey.Trim().ToLowerInvariant();

        IQueryable<Product> query = _products.Table.Where(p => p.Published && p.IsApprovedByAdmin);

        // Very small starter logic; replace with merchandising rules later.
        query = key switch
        {
            "top-picks" => query.OrderByDescending(p => p.Price),
            "winter-collections" => query.Where(p => p.Material != null).OrderByDescending(p => p.Id),
            _ => query.OrderByDescending(p => p.Id) // new-arrivals default
        };

        var items = await query.Take(12)
            .Select(p => new { p.Id, p.Name, p.Slug, p.Price, p.Currency })
            .ToListAsync(ct);

        var ids = items.Select(x => x.Id).ToList();
        var thumbs = await _media.Table
            .Where(m => ids.Contains(m.ProductId))
            .OrderBy(m => m.DisplayOrder)
            .GroupBy(m => m.ProductId)
            .Select(g => new { ProductId = g.Key, Url = g.Select(x => x.Url).FirstOrDefault() })
            .ToListAsync(ct);

        return items.Select(p => new ProductCard
        {
            ProductId = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Price = p.Price,
            Currency = p.Currency,
            ThumbnailUrl = thumbs.FirstOrDefault(t => t.ProductId == p.Id)?.Url
        }).ToList();
    }
}

