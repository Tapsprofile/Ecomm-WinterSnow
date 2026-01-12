namespace WinterSnow.Services.Notifications;

/// <summary>
/// Queue provider abstraction. Internal queue is the default in this scaffold.
/// </summary>
public interface INotificationQueue
{
    ValueTask EnqueueAsync(NotificationMessage message, CancellationToken ct = default);
    ValueTask<NotificationMessage> DequeueAsync(CancellationToken ct = default);
}

