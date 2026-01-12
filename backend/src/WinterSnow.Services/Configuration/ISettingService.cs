namespace WinterSnow.Services.Configuration;

public interface ISettingService
{
    Task<string?> GetAsync(string key, CancellationToken ct = default);
    Task UpsertAsync(string key, string value, CancellationToken ct = default);
    Task<Dictionary<string, string>> GetAllAsync(CancellationToken ct = default);
}

