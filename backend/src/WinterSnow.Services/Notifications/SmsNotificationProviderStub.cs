using WinterSnow.Core.Domain.Notifications;

namespace WinterSnow.Services.Notifications;

/// <summary>
/// Stubbed SMS provider. Replace with Twilio/MSG91/etc.
/// </summary>
public class SmsNotificationProviderStub : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.Sms;

    public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        // Intentionally no-op in scaffold.
        return Task.CompletedTask;
    }
}

