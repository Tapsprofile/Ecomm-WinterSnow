using System.Security.Cryptography;

namespace WinterSnow.Services.Payments.Providers;

public class RazorpayGatewayStub : IPaymentGateway
{
    public string SystemName => "razorpay";

    public Task<PaymentSessionResult> CreatePaymentSessionAsync(PaymentSessionRequest request, CancellationToken ct = default)
    {
        var bytes = RandomNumberGenerator.GetBytes(18);
        var sessionId = $"rzp_sess_{Convert.ToHexString(bytes).ToLowerInvariant()}";

        return Task.FromResult(new PaymentSessionResult
        {
            ProviderSystemName = "razorpay",
            ProviderDisplayName = "Razorpay",
            SessionId = sessionId
        });
    }
}

