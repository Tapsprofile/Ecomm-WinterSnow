using WinterSnow.Core.Domain.Notifications;
using WinterSnow.Data;

namespace WinterSnow.Services.Notifications;

public class InAppNotificationProvider : INotificationProvider
{
    private readonly WinterSnowDbContext _db;

    public InAppNotificationProvider(WinterSnowDbContext db)
    {
        _db = db;
    }

    public NotificationChannel Channel => NotificationChannel.InApp;

    public async Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        // Persist notification for UI inbox.
        _db.Notifications.Add(new Notification
        {
            RecipientType = message.RecipientType,
            RecipientUserId = message.RecipientUserId,
            RecipientVendorId = message.RecipientVendorId,
            Title = message.Title,
            Body = message.Body,
            ActionUrl = message.ActionUrl,
            IsRead = false
        });

        await _db.SaveChangesAsync(ct);
    }
}

