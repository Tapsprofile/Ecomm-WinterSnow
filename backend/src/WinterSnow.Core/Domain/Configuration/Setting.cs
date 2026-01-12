using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Configuration;

/// <summary>
/// Shopify-like settings foundation (key/value).
/// </summary>
public class Setting : BaseEntity
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;
}

