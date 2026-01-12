using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Payments;

public class VendorPayout : BaseEntity
{
    public int VendorId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";

    public DateTime PayoutDateUtc { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled|Processing|Paid|Failed
}

