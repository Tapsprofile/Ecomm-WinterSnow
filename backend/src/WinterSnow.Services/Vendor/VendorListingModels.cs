namespace WinterSnow.Services.Vendor;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public int? ParentCategoryId { get; set; }
}

public class CreateListingRequest
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? Material { get; set; }
    public int? CategoryId { get; set; }

    public decimal Price { get; set; }
    public bool AllowCoupons { get; set; }
    public bool IsVisibleInStorefront { get; set; } = true;
    public string ListingStatus { get; set; } = "Draft"; // Draft|Active|EndOfLife
    public List<string>? AllowedPostcodes { get; set; }

    public decimal? DiscountPercent { get; set; }
    public DateTime? DiscountStartUtc { get; set; }
    public DateTime? DiscountEndUtc { get; set; }

    public List<CreateListingVariant> Variants { get; set; } = [];
    public List<string> MediaUrls { get; set; } = [];
}

public class CreateListingVariant
{
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Sku { get; set; }
    public decimal? OverridePrice { get; set; }
    public int StockQuantity { get; set; }
}

public class UpdateListingRequest
{
    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? Material { get; set; }
    public int? CategoryId { get; set; }

    public decimal Price { get; set; }
    public bool AllowCoupons { get; set; }
    public bool IsVisibleInStorefront { get; set; }
    public string ListingStatus { get; set; } = "Draft"; // Draft|Active|EndOfLife

    /// <summary>
    /// If provided, replaces allowed postcode list (used to restrict visibility).
    /// If empty list, restrictions are cleared.
    /// </summary>
    public List<string>? AllowedPostcodes { get; set; }

    public decimal? DiscountPercent { get; set; }
    public DateTime? DiscountStartUtc { get; set; }
    public DateTime? DiscountEndUtc { get; set; }
}

