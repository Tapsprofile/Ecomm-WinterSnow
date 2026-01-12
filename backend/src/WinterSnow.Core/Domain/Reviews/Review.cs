using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Reviews;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }

    public int Rating { get; set; } // 1-5
    public required string Title { get; set; }
    public required string ReviewText { get; set; }

    public bool IsVerifiedPurchase { get; set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

