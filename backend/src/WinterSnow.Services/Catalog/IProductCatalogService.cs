namespace WinterSnow.Services.Catalog;

public interface IProductCatalogService
{
    Task<ProductDetails?> GetProductBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<ProductCard>> GetHomepageSectionAsync(string sectionKey, CancellationToken ct = default);
}

public class ProductCard
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "INR";
    public string? ThumbnailUrl { get; set; }
}

