using WinterSnow.Core.Domain.Notifications;

namespace WinterSnow.Services.Notifications;

/// <summary>
/// Stubbed email provider. Replace with SMTP/SendGrid/etc.
/// </summary>
public class EmailNotificationProviderStub : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        // Intentionally no-op in scaffold.
        return Task.CompletedTask;
    }
}

