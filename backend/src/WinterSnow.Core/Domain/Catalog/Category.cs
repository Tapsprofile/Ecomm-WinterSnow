using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Catalog;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string? Slug { get; set; }

    public int? ParentCategoryId { get; set; }
}

