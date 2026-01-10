using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Payments;

public class VendorLedgerEntry : BaseEntity
{
    public int VendorId { get; set; }
    public int? OrderId { get; set; }

    /// <summary>
    /// Positive for credit, negative for debit.
    /// </summary>
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "INR";
    public required string Description { get; set; }

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

