using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Catalog;

public class ProductMedia : BaseEntity
{
    public int ProductId { get; set; }

    public required string Url { get; set; }
    public string MediaType { get; set; } = "image"; // image|video
    public int DisplayOrder { get; set; }

    public int? MediaAttachmentId { get; set; }
}

