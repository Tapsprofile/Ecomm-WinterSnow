using WinterSnow.Core.Domain.Common;

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

    public bool Published { get; set; }
    public bool IsApprovedByAdmin { get; set; }

    public int VendorId { get; set; }

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

