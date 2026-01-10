using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Catalog;

/// <summary>
/// Variant management (size/color).
/// </summary>
public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }

    public string? Size { get; set; }
    public string? Color { get; set; }

    public string? Sku { get; set; }

    public decimal? OverridePrice { get; set; }
    public int StockQuantity { get; set; }
}

