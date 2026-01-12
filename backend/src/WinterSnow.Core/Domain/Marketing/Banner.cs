using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Marketing;

public class Banner : BaseEntity
{
    public required string Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? TargetUrl { get; set; }

    public bool IsActive { get; set; }
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}

