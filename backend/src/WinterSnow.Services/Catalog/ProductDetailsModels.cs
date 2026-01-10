namespace WinterSnow.Services.Catalog;

public class ProductDetails
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public string? Material { get; set; }
    public int? CategoryId { get; set; }

    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public bool AllowCoupons { get; set; }
    public string Currency { get; set; } = "INR";

    public List<ProductMediaDto> Media { get; set; } = [];
    public List<ProductVariantDto> Variants { get; set; } = [];
    public List<ReviewDto> Reviews { get; set; } = [];
}

public class ProductMediaDto
{
    public required string Url { get; set; }
    public string MediaType { get; set; } = "image";
    public int DisplayOrder { get; set; }
}

public class ProductVariantDto
{
    public int VariantId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Sku { get; set; }
    public decimal EffectivePrice { get; set; }
    public int StockQuantity { get; set; }
}

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int Rating { get; set; }
    public required string Title { get; set; }
    public required string ReviewText { get; set; }
    public bool IsVerifiedPurchase { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public List<string> MediaUrls { get; set; } = [];
}

