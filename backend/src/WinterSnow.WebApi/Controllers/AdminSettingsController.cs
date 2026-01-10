using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Configuration;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/settings")]
public class AdminSettingsController : ControllerBase
{
    private readonly ISettingService _settings;

    public AdminSettingsController(ISettingService settings)
    {
        _settings = settings;
    }

    [HttpGet]
    public Task<Dictionary<string, string>> GetAll(CancellationToken ct)
        => _settings.GetAllAsync(ct);

    public class UpsertSettingRequest
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertSettingRequest req, CancellationToken ct)
    {
        await _settings.UpsertAsync(req.Key, req.Value, ct);
        return NoContent();
    }
}

