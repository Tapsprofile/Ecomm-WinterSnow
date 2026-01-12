namespace WinterSnow.Services.Webhooks;

public interface IWebhookAdminService
{
    Task<List<WebhookSubscriptionDto>> ListAsync(CancellationToken ct = default);
    Task<int> CreateAsync(CreateWebhookSubscriptionRequest request, CancellationToken ct = default);
    Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
}

public class WebhookSubscriptionDto
{
    public int Id { get; set; }
    public required string EventName { get; set; }
    public required string TargetUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}

