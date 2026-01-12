using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Notifications;

/// <summary>
/// Persisted in-app notification (email/sms deliveries are tracked separately).
/// </summary>
public class Notification : BaseEntity
{
    public NotificationRecipientType RecipientType { get; set; }

    /// <summary>
    /// For Customer/Admin recipients.
    /// </summary>
    public int? RecipientUserId { get; set; }

    /// <summary>
    /// For Vendor recipients (vendor cockpit notifications).
    /// </summary>
    public int? RecipientVendorId { get; set; }

    public required string Title { get; set; }
    public required string Body { get; set; }

    /// <summary>
    /// Optional deep link (UI can navigate).
    /// </summary>
    public string? ActionUrl { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReadOnUtc { get; set; }
}

