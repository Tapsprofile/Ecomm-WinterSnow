namespace WinterSnow.Services.Payments.Providers;

public class PaymentSessionResult
{
    public required string ProviderSystemName { get; set; }
    public required string ProviderDisplayName { get; set; }
    public required string SessionId { get; set; }
}

