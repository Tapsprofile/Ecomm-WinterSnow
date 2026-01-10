namespace WinterSnow.Services.Payments;

public class PaymentTransactionQuery
{
    public int? TransactionId { get; set; }
    public Guid? PaymentGroupId { get; set; }
    public int? VendorId { get; set; }
    public int? OrderId { get; set; }

    public int? ProviderId { get; set; }
    public string? ProviderSystemName { get; set; }

    public string? ProviderOrderId { get; set; }
    public string? ProviderPaymentId { get; set; }
    public string? ProviderPaymentSessionId { get; set; }

    public string? Status { get; set; } // Pending/Paid/Failed/...

    public int Take { get; set; } = 100;
}

public class PaymentTransactionDto
{
    public int TransactionId { get; set; }
    public Guid PaymentGroupId { get; set; }
    public int OrderId { get; set; }
    public int VendorId { get; set; }

    public int PaymentProviderId { get; set; }
    public required string PaymentProviderSystemName { get; set; }
    public required string PaymentProviderDisplayName { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ProviderOrderId { get; set; }
    public string? ProviderPaymentId { get; set; }
    public string? ProviderPaymentSessionId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTime CreatedOnUtc { get; set; }
}

