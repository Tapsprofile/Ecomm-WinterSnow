namespace WinterSnow.Services.Payments.Providers;

public interface IPaymentGateway
{
    /// <summary>
    /// Matches PaymentProvider.SystemName (e.g. "cashfree").
    /// </summary>
    string SystemName { get; }

    Task<PaymentSessionResult> CreatePaymentSessionAsync(PaymentSessionRequest request, CancellationToken ct = default);
}

public class PaymentSessionRequest
{
    public required string CustomerEmail { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
}

