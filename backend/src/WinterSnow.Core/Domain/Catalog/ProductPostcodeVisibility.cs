using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Catalog;

/// <summary>
/// If a product has any entries here, it is only visible for those postcodes.
/// </summary>
public class ProductPostcodeVisibility : BaseEntity
{
    public int ProductId { get; set; }
    public required string PostalCode { get; set; }
}

