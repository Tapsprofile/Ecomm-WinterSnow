using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Auditing;

public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string? Role { get; set; }

    public required string Action { get; set; } // e.g. "Vendor.CreateListing"
    public string? EntityName { get; set; }     // e.g. "Product"
    public int? EntityId { get; set; }

    public string? Path { get; set; }
    public string? Method { get; set; }
    public int? StatusCode { get; set; }

    public string? MetadataJson { get; set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

