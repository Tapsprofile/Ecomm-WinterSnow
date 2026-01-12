using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Marketing;

public class FlashSaleEvent : BaseEntity
{
    public required string Name { get; set; }
    public decimal DiscountPercent { get; set; } // 0-100
    public bool IsActive { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}

