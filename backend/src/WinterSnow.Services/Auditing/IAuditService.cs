namespace WinterSnow.Services.Auditing;

public interface IAuditService
{
    Task WriteAsync(AuditEntry entry, CancellationToken ct = default);
}

public class AuditEntry
{
    public int? UserId { get; set; }
    public string? Role { get; set; }
    public required string Action { get; set; }
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public int? StatusCode { get; set; }
    public string? MetadataJson { get; set; }
}

