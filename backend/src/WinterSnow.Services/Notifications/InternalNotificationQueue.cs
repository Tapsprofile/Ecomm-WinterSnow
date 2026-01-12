using System.Threading.Channels;

namespace WinterSnow.Services.Notifications;

/// <summary>
/// Internal in-memory queue provider (no external MQ).
/// </summary>
public class InternalNotificationQueue : INotificationQueue
{
    private readonly Channel<NotificationMessage> _channel;

    public InternalNotificationQueue()
    {
        _channel = Channel.CreateBounded<NotificationMessage>(new BoundedChannelOptions(500)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
    }

    public ValueTask EnqueueAsync(NotificationMessage message, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(message, ct);

    public ValueTask<NotificationMessage> DequeueAsync(CancellationToken ct = default)
        => _channel.Reader.ReadAsync(ct);
}

