using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Customers;

public class Vendor : BaseEntity
{
    public required string Name { get; set; }
    public string? DisplayName { get; set; }

    /// <summary>
    /// KYC status for onboarding / moderation.
    /// </summary>
    public bool IsKycApproved { get; set; }

    public bool IsActive { get; set; }

    /// <summary>
    /// Commission rate (0.10 - 0.20 typically).
    /// </summary>
    public decimal CommissionRate { get; set; }
}

