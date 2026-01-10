using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Orders;

/// <summary>
/// Root order for a single vendor (split-order orchestration creates multiple Orders per checkout).
/// </summary>
public class Order : BaseEntity
{
    public int CustomerId { get; set; }
    public int VendorId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public int ShippingAddressId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal OrderTotal { get; set; }

    public string Currency { get; set; } = "INR";

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

