using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Orders;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }

    public required string ProductName { get; set; }

    public decimal UnitPriceInclTax { get; set; }
    public int Quantity { get; set; }
    public decimal PriceInclTax { get; set; }
}

