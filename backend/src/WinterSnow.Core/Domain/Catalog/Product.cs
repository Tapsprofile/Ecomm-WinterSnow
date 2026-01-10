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

    public decimal Price { get; set; }
    public string Currency { get; set; } = "INR";

    public bool Published { get; set; }
    public bool IsApprovedByAdmin { get; set; }

    public int VendorId { get; set; }

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}

