using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Payments.Providers;

/// <summary>
/// Vendor-specific payment provider routing.
/// This enables marketplaces where vendors may prefer different providers.
/// </summary>
public class VendorPaymentProvider : BaseEntity
{
    public int VendorId { get; set; }
    public int PaymentProviderId { get; set; }

    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0; // lower = preferred
}

