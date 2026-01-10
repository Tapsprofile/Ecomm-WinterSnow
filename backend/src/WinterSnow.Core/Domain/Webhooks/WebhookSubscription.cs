using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Webhooks;

public class WebhookSubscription : BaseEntity
{
    public required string EventName { get; set; } // e.g. order.created, payment.paid
    public required string TargetUrl { get; set; }
    public string? Secret { get; set; } // used to sign payloads (HMAC)

    public bool IsActive { get; set; } = true;
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

