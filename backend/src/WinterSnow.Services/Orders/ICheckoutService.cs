namespace WinterSnow.Services.Orders;

public interface ICheckoutService
{
    Task<List<SplitOrderSummary>> PreviewSplitAsync(List<CheckoutItem> items, CancellationToken ct = default);
    Task<CheckoutResult> CreateOrdersAndPaymentSessionAsync(int customerId, CheckoutRequest request, CancellationToken ct = default);
}

