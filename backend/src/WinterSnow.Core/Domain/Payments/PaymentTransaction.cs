using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Payments;

public class PaymentTransaction : BaseEntity
{
    /// <summary>
    /// Groups multiple transactions created in the same checkout submit.
    /// Useful when a cart becomes multiple vendor orders and possibly multiple providers.
    /// </summary>
    public Guid PaymentGroupId { get; set; }

    public int OrderId { get; set; }
    public int VendorId { get; set; }

    public int PaymentProviderId { get; set; }
    public string PaymentProviderSystemName { get; set; } = "cashfree";
    public string PaymentProviderDisplayName { get; set; } = "Cashfree";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// Gateway identifiers (searchable).
    /// </summary>
    public string? ProviderOrderId { get; set; }
    public string? ProviderPaymentId { get; set; }

    /// <summary>
    /// Session/token returned by gateway for client-side payment initiation.
    /// </summary>
    public string? ProviderPaymentSessionId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

