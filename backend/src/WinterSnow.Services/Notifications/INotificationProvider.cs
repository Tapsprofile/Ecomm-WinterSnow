using WinterSnow.Core.Domain.Notifications;

namespace WinterSnow.Services.Notifications;

public interface INotificationProvider
{
    NotificationChannel Channel { get; }
    Task SendAsync(NotificationMessage message, CancellationToken ct = default);
}

