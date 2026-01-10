namespace WinterSnow.Services.Payments.Providers;

public interface IPaymentGatewayRegistry
{
    IPaymentGateway GetRequired(string providerSystemName);
    bool TryGet(string providerSystemName, out IPaymentGateway gateway);
}

public class PaymentGatewayRegistry : IPaymentGatewayRegistry
{
    private readonly Dictionary<string, IPaymentGateway> _bySystemName;

    public PaymentGatewayRegistry(IEnumerable<IPaymentGateway> gateways)
    {
        _bySystemName = gateways.ToDictionary(g => g.SystemName, StringComparer.OrdinalIgnoreCase);
    }

    public IPaymentGateway GetRequired(string providerSystemName)
    {
        if (TryGet(providerSystemName, out var gw))
            return gw;
        throw new InvalidOperationException($"No payment gateway registered for '{providerSystemName}'.");
    }

    public bool TryGet(string providerSystemName, out IPaymentGateway gateway)
        => _bySystemName.TryGetValue(providerSystemName, out gateway!);
}

