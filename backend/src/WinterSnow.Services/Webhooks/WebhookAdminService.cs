using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Webhooks;
using WinterSnow.Data;

namespace WinterSnow.Services.Webhooks;

public class WebhookAdminService : IWebhookAdminService
{
    private readonly WinterSnowDbContext _db;

    public WebhookAdminService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<List<WebhookSubscriptionDto>> ListAsync(CancellationToken ct = default)
    {
        return await _db.WebhookSubscriptions
            .OrderByDescending(x => x.Id)
            .Select(x => new WebhookSubscriptionDto
            {
                Id = x.Id,
                EventName = x.EventName,
                TargetUrl = x.TargetUrl,
                IsActive = x.IsActive,
                CreatedOnUtc = x.CreatedOnUtc
            })
            .ToListAsync(ct);
    }

    public async Task<int> CreateAsync(CreateWebhookSubscriptionRequest request, CancellationToken ct = default)
    {
        var sub = new WebhookSubscription
        {
            EventName = request.EventName.Trim(),
            TargetUrl = request.TargetUrl.Trim(),
            Secret = string.IsNullOrWhiteSpace(request.Secret) ? null : request.Secret.Trim(),
            IsActive = request.IsActive
        };
        _db.WebhookSubscriptions.Add(sub);
        await _db.SaveChangesAsync(ct);
        return sub.Id;
    }

    public async Task SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var sub = await _db.WebhookSubscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (sub is null)
            return;
        sub.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }
}

