using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Core.Domain.Orders;

public class Address : BaseEntity
{
    public required string FullName { get; set; }
    public required string Line1 { get; set; }
    public string? Line2 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string CountryCode { get; set; } = "IN";

    public string? Phone { get; set; }
}

