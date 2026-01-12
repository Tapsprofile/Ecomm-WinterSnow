namespace WinterSnow.Services.Webhooks;

public interface IWebhookDispatcher
{
    Task DispatchAsync(string eventName, object payload, CancellationToken ct = default);
}

