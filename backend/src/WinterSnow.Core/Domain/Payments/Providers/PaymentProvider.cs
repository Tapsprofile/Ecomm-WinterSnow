using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Payments.Providers;

public class PaymentProvider : BaseEntity
{
    /// <summary>
    /// Stable identifier used by code (e.g. "cashfree", "razorpay").
    /// </summary>
    public required string SystemName { get; set; }

    public required string DisplayName { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Provider configuration (API keys etc). In production use secrets vault, not plain DB.
    /// </summary>
    public string? ConfigJson { get; set; }
}

