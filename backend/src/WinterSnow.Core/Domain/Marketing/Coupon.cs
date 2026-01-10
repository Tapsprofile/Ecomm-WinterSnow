using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Marketing;

public class Coupon : BaseEntity
{
    public required string Code { get; set; }
    public string? Description { get; set; }

    public decimal DiscountAmount { get; set; }
    public bool IsActive { get; set; }

    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}

