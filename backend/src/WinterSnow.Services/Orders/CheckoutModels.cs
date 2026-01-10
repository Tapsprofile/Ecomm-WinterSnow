namespace WinterSnow.Services.Orders;

public class CheckoutItem
{
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class CheckoutRequest
{
    public required ShippingAddressInput ShippingAddress { get; set; }
    public List<CheckoutItem> Items { get; set; } = [];
    public string? CouponCode { get; set; }
}

public class CheckoutPreviewRequestV2
{
    public List<CheckoutItem> Items { get; set; } = [];
    public string? CouponCode { get; set; }
}

public class ShippingAddressInput
{
    public required string FullName { get; set; }
    public required string Line1 { get; set; }
    public string? Line2 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string CountryCode { get; set; } = "IN";
    public string? Phone { get; set; }
}

public class AddressValidationResult
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public ShippingAddressInput? Normalized { get; set; }
}

public class SplitOrderSummary
{
    public int VendorId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal OrderTotal { get; set; }
    public decimal OrderTotalAfterDiscount { get; set; }
    public string Currency { get; set; } = "INR";
    public List<SplitOrderItem> Items { get; set; } = [];
}

public class SplitOrderItem
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CheckoutResult
{
    public List<int> CreatedOrderIds { get; set; } = [];
    public List<SplitOrderSummary> SplitSummary { get; set; } = [];

    /// <summary>
    /// Payment session for Cashfree modal (stubbed).
    /// </summary>
    public string? PaymentSessionId { get; set; }
}

