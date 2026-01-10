using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Payments;

public class PaymentTransaction : BaseEntity
{
    public int OrderId { get; set; }
    public string Provider { get; set; } = "Cashfree";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? ProviderOrderId { get; set; }
    public string? ProviderPaymentId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

