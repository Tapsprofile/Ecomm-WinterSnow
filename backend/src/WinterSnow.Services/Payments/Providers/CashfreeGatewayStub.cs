using System.Security.Cryptography;

namespace WinterSnow.Services.Payments.Providers;

public class CashfreeGatewayStub : IPaymentGateway
{
    public string SystemName => "cashfree";

    public Task<PaymentSessionResult> CreatePaymentSessionAsync(PaymentSessionRequest request, CancellationToken ct = default)
    {
        var bytes = RandomNumberGenerator.GetBytes(18);
        var sessionId = $"cf_sess_{Convert.ToHexString(bytes).ToLowerInvariant()}";

        return Task.FromResult(new PaymentSessionResult
        {
            ProviderSystemName = "cashfree",
            ProviderDisplayName = "Cashfree",
            SessionId = sessionId
        });
    }
}

