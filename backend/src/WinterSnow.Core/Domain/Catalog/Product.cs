using WinterSnow.Core.Domain.Common;
using WinterSnow.Core.Domain.Catalog.Listings;

namespace WinterSnow.Core.Domain.Catalog;

/// <summary>
/// Universal Product Catalog (UPC) entry.
/// </summary>
public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }

    // Discovery facets
    public string? Material { get; set; }

    /// <summary>
    /// Category selection (Amazon-like taxonomy path can be built via Category.ParentCategoryId).
    /// </summary>
    public int? CategoryId { get; set; }

    public decimal Price { get; set; }
    public string Currency { get; set; } = "INR";

    /// <summary>
    /// eBay-style seller discount info on the listing itself.
    /// </summary>
    public decimal? DiscountPercent { get; set; } // 0-100
    public DateTime? DiscountStartUtc { get; set; }
    public DateTime? DiscountEndUtc { get; set; }

    /// <summary>
    /// Whether coupons can be applied to this listing at checkout.
    /// </summary>
    public bool AllowCoupons { get; set; }

    /// <summary>
    /// Vendor lifecycle control:
    /// - Draft: internal/vendor-only, not shown to customers
    /// - Active: can be shown (subject to approvals/stock/visibility rules)
    /// - EndOfLife: removed from customer listings; vendor can restore later
    /// </summary>
    public ListingStatus ListingStatus { get; set; } = ListingStatus.Draft;

    /// <summary>
    /// Vendor can keep listing internal (not visible to customers) even if approved.
    /// </summary>
    public bool IsVisibleInStorefront { get; set; } = true;

    public bool Published { get; set; }
    public bool IsApprovedByAdmin { get; set; }

    public int VendorId { get; set; }

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

