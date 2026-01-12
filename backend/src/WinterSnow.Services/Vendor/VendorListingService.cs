using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Catalog.Listings;
using WinterSnow.Data;

namespace WinterSnow.Services.Vendor;

public class VendorListingService : IVendorListingService
{
    private readonly WinterSnowDbContext _db;

    public VendorListingService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        // If none exist yet, seed a starter taxonomy.
        if (!await _db.Categories.AnyAsync(ct))
        {
            var root = new Category { Name = "Winter Gear", Slug = "winter-gear", ParentCategoryId = null };
            _db.Categories.Add(root);
            await _db.SaveChangesAsync(ct);

            _db.Categories.AddRange(
                new Category { Name = "Jackets", Slug = "jackets", ParentCategoryId = root.Id },
                new Category { Name = "Gloves", Slug = "gloves", ParentCategoryId = root.Id },
                new Category { Name = "Boots", Slug = "boots", ParentCategoryId = root.Id },
                new Category { Name = "Accessories", Slug = "accessories", ParentCategoryId = root.Id }
            );
            await _db.SaveChangesAsync(ct);
        }

        return await _db.Categories
            .OrderBy(c => c.ParentCategoryId)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                CategoryId = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId
            })
            .ToListAsync(ct);
    }

    public async Task<int> CreateListingAsync(int vendorId, CreateListingRequest request, CancellationToken ct = default)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();
        if (await _db.Products.AnyAsync(p => p.Slug.ToLower() == slug, ct))
            throw new InvalidOperationException("Slug already exists.");

        if (request.DiscountPercent is not null && (request.DiscountPercent < 0 || request.DiscountPercent > 100))
            throw new InvalidOperationException("DiscountPercent must be between 0 and 100.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Slug = slug,
            ShortDescription = request.ShortDescription,
            FullDescription = request.FullDescription,
            Material = request.Material,
            CategoryId = request.CategoryId,
            Price = request.Price,
            Currency = "INR",
            VendorId = vendorId,
            AllowCoupons = request.AllowCoupons,
            IsVisibleInStorefront = request.IsVisibleInStorefront,
            DiscountPercent = request.DiscountPercent,
            DiscountStartUtc = request.DiscountStartUtc,
            DiscountEndUtc = request.DiscountEndUtc,
            Published = false,
            IsApprovedByAdmin = false
        };

        if (Enum.TryParse<ListingStatus>(request.ListingStatus, true, out var st))
            product.ListingStatus = st;

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        if (request.AllowedPostcodes is not null)
        {
            var normalized = request.AllowedPostcodes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var pc in normalized)
            {
                _db.ProductPostcodeVisibilities.Add(new ProductPostcodeVisibility
                {
                    ProductId = product.Id,
                    PostalCode = pc
                });
            }
        }

        // Variants (inventory per size/color)
        var variants = request.Variants.Count == 0
            ? new List<CreateListingVariant> { new() { Size = "Default", Color = "Default", StockQuantity = 0 } }
            : request.Variants;

        foreach (var v in variants)
        {
            _db.ProductVariants.Add(new ProductVariant
            {
                ProductId = product.Id,
                Size = v.Size,
                Color = v.Color,
                Sku = v.Sku,
                OverridePrice = v.OverridePrice,
                StockQuantity = Math.Max(0, v.StockQuantity)
            });
        }

        // Media URLs (simple; replace with real uploads later)
        var order = 1;
        foreach (var url in request.MediaUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
        {
            _db.ProductMedia.Add(new ProductMedia
            {
                ProductId = product.Id,
                Url = url.Trim(),
                DisplayOrder = order++
            });
        }

        await _db.SaveChangesAsync(ct);
        return product.Id;
    }

    public async Task UpdateListingAsync(int vendorId, int productId, UpdateListingRequest request, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId && p.VendorId == vendorId, ct);
        if (product is null)
            throw new InvalidOperationException("Listing not found.");

        if (request.DiscountPercent is not null && (request.DiscountPercent < 0 || request.DiscountPercent > 100))
            throw new InvalidOperationException("DiscountPercent must be between 0 and 100.");

        product.Name = request.Name.Trim();
        product.ShortDescription = request.ShortDescription;
        product.FullDescription = request.FullDescription;
        product.Material = request.Material;
        product.CategoryId = request.CategoryId;
        product.Price = request.Price;
        product.AllowCoupons = request.AllowCoupons;
        product.IsVisibleInStorefront = request.IsVisibleInStorefront;
        product.DiscountPercent = request.DiscountPercent;
        product.DiscountStartUtc = request.DiscountStartUtc;
        product.DiscountEndUtc = request.DiscountEndUtc;

        if (Enum.TryParse<ListingStatus>(request.ListingStatus, true, out var st))
            product.ListingStatus = st;

        // Postcode restrictions: replace list when provided
        if (request.AllowedPostcodes is not null)
        {
            var normalized = request.AllowedPostcodes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existing = await _db.ProductPostcodeVisibilities.Where(x => x.ProductId == productId).ToListAsync(ct);
            _db.ProductPostcodeVisibilities.RemoveRange(existing);

            foreach (var pc in normalized)
            {
                _db.ProductPostcodeVisibilities.Add(new ProductPostcodeVisibility
                {
                    ProductId = productId,
                    PostalCode = pc
                });
            }
        }

        // If vendor changes listing details, keep approval as-is, but you may choose to re-approve.
        await _db.SaveChangesAsync(ct);
    }
}

