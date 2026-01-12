using WinterSnow.Core.Domain.Notifications;

namespace WinterSnow.Services.Notifications;

public class NotificationMessage
{
    public NotificationRecipientType RecipientType { get; set; }
    public int? RecipientUserId { get; set; }
    public int? RecipientVendorId { get; set; }

    public required string Title { get; set; }
    public required string Body { get; set; }
    public string? ActionUrl { get; set; }

    public List<NotificationChannel> Channels { get; set; } = [NotificationChannel.InApp];

    // Optional contact overrides (for email/sms)
    public string? EmailTo { get; set; }
    public string? PhoneTo { get; set; }
}

