using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Media;

public class MediaAttachment : BaseEntity
{
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long Length { get; set; }

    /// <summary>
    /// Local or CDN URL.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Internal storage path on server (for local storage).
    /// </summary>
    public required string StoragePath { get; set; }

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}

