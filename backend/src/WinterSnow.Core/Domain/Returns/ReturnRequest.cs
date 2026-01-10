using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Returns;

public class ReturnRequest : BaseEntity
{
    public int OrderId { get; set; }
    public int OrderItemId { get; set; }
    public int CustomerId { get; set; }
    public int VendorId { get; set; }

    public required string Reason { get; set; }
    public string? Notes { get; set; }

    public ReturnStatus Status { get; set; } = ReturnStatus.Requested;

    /// <summary>
    /// Placeholder for shipping label URL (reverse logistics).
    /// </summary>
    public string? ReturnLabelUrl { get; set; }

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

