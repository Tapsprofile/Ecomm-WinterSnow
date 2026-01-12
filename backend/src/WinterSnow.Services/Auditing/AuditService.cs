using WinterSnow.Core.Domain.Auditing;
using WinterSnow.Data;

namespace WinterSnow.Services.Auditing;

public class AuditService : IAuditService
{
    private readonly WinterSnowDbContext _db;

    public AuditService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(AuditEntry entry, CancellationToken ct = default)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = entry.UserId,
            Role = entry.Role,
            Action = entry.Action,
            EntityName = entry.EntityName,
            EntityId = entry.EntityId,
            Path = entry.Path,
            Method = entry.Method,
            StatusCode = entry.StatusCode,
            MetadataJson = entry.MetadataJson
        });

        await _db.SaveChangesAsync(ct);
    }
}

