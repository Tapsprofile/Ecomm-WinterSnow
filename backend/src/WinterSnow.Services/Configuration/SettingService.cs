using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Configuration;
using WinterSnow.Data;

namespace WinterSnow.Services.Configuration;

public class SettingService : ISettingService
{
    private readonly WinterSnowDbContext _db;

    public SettingService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetAsync(string key, CancellationToken ct = default)
    {
        var k = key.Trim();
        var s = await _db.Settings.FirstOrDefaultAsync(x => x.Key == k, ct);
        return s?.Value;
    }

    public async Task<Dictionary<string, string>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Settings
            .OrderBy(x => x.Key)
            .ToDictionaryAsync(x => x.Key, x => x.Value, ct);
    }

    public async Task UpsertAsync(string key, string value, CancellationToken ct = default)
    {
        var k = key.Trim();
        var v = value ?? string.Empty;

        var existing = await _db.Settings.FirstOrDefaultAsync(x => x.Key == k, ct);
        if (existing is null)
        {
            _db.Settings.Add(new Setting { Key = k, Value = v, UpdatedOnUtc = DateTime.UtcNow });
        }
        else
        {
            existing.Value = v;
            existing.UpdatedOnUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }
}

