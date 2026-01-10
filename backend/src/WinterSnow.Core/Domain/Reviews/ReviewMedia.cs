using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Reviews;

public class ReviewMedia : BaseEntity
{
    public int ReviewId { get; set; }
    public required string Url { get; set; }
    public string MediaType { get; set; } = "image";
}

