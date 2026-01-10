using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using WinterSnow.Data;

namespace WinterSnow.Services.Webhooks;

/// <summary>
/// Lightweight webhook dispatcher (Shopify-style integration primitive).
/// In production, move delivery to a background queue and implement retries/backoff.
/// </summary>
public class WebhookDispatcher : IWebhookDispatcher
{
    private readonly WinterSnowDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebhookDispatcher(WinterSnowDbContext db, IHttpClientFactory httpClientFactory)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
    }

    public async Task DispatchAsync(string eventName, object payload, CancellationToken ct = default)
    {
        var subs = await _db.WebhookSubscriptions
            .Where(s => s.IsActive && s.EventName == eventName)
            .ToListAsync(ct);

        if (subs.Count == 0)
            return;

        var client = _httpClientFactory.CreateClient();
        var json = global::System.Text.Json.JsonSerializer.Serialize(payload);

        foreach (var s in subs)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, s.TargetUrl)
            {
                Content = new StringContent(json, global::System.Text.Encoding.UTF8, "application/json")
            };

            req.Headers.Add("X-WinterSnow-Event", eventName);

            if (!string.IsNullOrWhiteSpace(s.Secret))
            {
                var sig = Sign(json, s.Secret);
                req.Headers.Add("X-WinterSnow-Signature", sig);
            }

            // Fire-and-forget style per-subscription attempt (no retries here).
            try
            {
                using var resp = await client.SendAsync(req, ct);
                _ = resp.StatusCode;
            }
            catch
            {
                // swallow in scaffold; production should log + retry
            }
        }
    }

    private static string Sign(string body, string secret)
    {
        using var hmac = new HMACSHA256(global::System.Text.Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(global::System.Text.Encoding.UTF8.GetBytes(body));
        return global::System.Convert.ToHexString(hash).ToLowerInvariant();
    }
}

