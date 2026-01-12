namespace WinterSnow.Services.Customers;

public class CustomerOrderListItem
{
    public int OrderId { get; set; }
    public int VendorId { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal OrderTotal { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTime CreatedOnUtc { get; set; }
    public int ItemsCount { get; set; }
}

public class CustomerOrderItemDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal UnitPriceInclTax { get; set; }
    public int Quantity { get; set; }
    public decimal PriceInclTax { get; set; }
}

public class CustomerOrderAddressDto
{
    public string FullName { get; set; } = "";
    public string Line1 { get; set; } = "";
    public string? Line2 { get; set; }
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string CountryCode { get; set; } = "IN";
    public string? Phone { get; set; }
}

public class CustomerOrderDetailsDto
{
    public int OrderId { get; set; }
    public int VendorId { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal OrderTotal { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTime CreatedOnUtc { get; set; }
    public CustomerOrderAddressDto ShippingAddress { get; set; } = new();
    public List<CustomerOrderItemDto> Items { get; set; } = new();
}

