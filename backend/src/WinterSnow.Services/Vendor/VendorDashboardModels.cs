namespace WinterSnow.Services.Vendor;

public class VendorSalesOverview
{
    public decimal RevenueToday { get; set; }
    public decimal RevenueThisWeek { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<TopProduct> TopWinterProducts { get; set; } = [];
}

public class TopProduct
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
}

public class VendorInventoryItem
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public decimal Price { get; set; }
    public bool Published { get; set; }
    public bool IsApprovedByAdmin { get; set; }
    public List<InventoryVariant> Variants { get; set; } = [];
}

public class InventoryVariant
{
    public int VariantId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public decimal? OverridePrice { get; set; }
    public int StockQuantity { get; set; }
}

public class BulkUpdateVariantRequest
{
    public int VariantId { get; set; }
    public decimal? OverridePrice { get; set; }
    public int? StockQuantity { get; set; }
}

public class BulkUpdateInventoryRequest
{
    public List<BulkUpdateVariantRequest> Variants { get; set; } = [];
}

