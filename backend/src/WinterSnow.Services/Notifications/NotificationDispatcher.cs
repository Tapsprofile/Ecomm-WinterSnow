using WinterSnow.Core.Domain.Notifications;

namespace WinterSnow.Services.Notifications;

public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationMessage message, CancellationToken ct = default);
}

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IReadOnlyDictionary<NotificationChannel, INotificationProvider> _providers;

    public NotificationDispatcher(IEnumerable<INotificationProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.Channel);
    }

    public async Task DispatchAsync(NotificationMessage message, CancellationToken ct = default)
    {
        foreach (var channel in message.Channels.Distinct())
        {
            if (_providers.TryGetValue(channel, out var provider))
                await provider.SendAsync(message, ct);
        }
    }
}

