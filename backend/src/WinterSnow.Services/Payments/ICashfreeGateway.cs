namespace WinterSnow.Services.Payments;

public interface ICashfreeGateway
{
    Task<string> CreatePaymentSessionAsync(decimal amount, string currency, string customerEmail, CancellationToken ct = default);
}

