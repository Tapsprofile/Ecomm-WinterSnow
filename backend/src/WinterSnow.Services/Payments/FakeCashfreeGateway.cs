using System.Security.Cryptography;

namespace WinterSnow.Services.Payments;

/// <summary>
/// Dev-only stub for Cashfree modal/session integration.
/// Replace with real Cashfree SDK calls.
/// </summary>
public class FakeCashfreeGateway : ICashfreeGateway
{
    public Task<string> CreatePaymentSessionAsync(decimal amount, string currency, string customerEmail, CancellationToken ct = default)
    {
        // Fake session id that looks like an external token.
        var bytes = RandomNumberGenerator.GetBytes(18);
        return Task.FromResult($"cf_sess_{Convert.ToHexString(bytes).ToLowerInvariant()}");
    }
}

