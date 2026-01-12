namespace WinterSnow.Services.Webhooks;

public class CreateWebhookSubscriptionRequest
{
    public required string EventName { get; set; }
    public required string TargetUrl { get; set; }
    public string? Secret { get; set; }
    public bool IsActive { get; set; } = true;
}

