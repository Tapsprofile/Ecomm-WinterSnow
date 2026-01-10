namespace WinterSnow.Services.Discovery;

public class SearchQuery
{
    public string? Q { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Material { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 24;
}

public class FacetBucket
{
    public required string Value { get; set; }
    public int Count { get; set; }
}

public class SearchFacets
{
    public List<FacetBucket> Sizes { get; set; } = [];
    public List<FacetBucket> Colors { get; set; } = [];
    public List<FacetBucket> Materials { get; set; } = [];
}

public class SearchResultItem
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public bool AllowCoupons { get; set; }
    public string Currency { get; set; } = "INR";
    public string? ThumbnailUrl { get; set; }
    public double RatingAvg { get; set; }
    public int RatingCount { get; set; }
}

public class SearchResponse
{
    public List<SearchResultItem> Items { get; set; } = [];
    public int Total { get; set; }
    public SearchFacets Facets { get; set; } = new();
}

