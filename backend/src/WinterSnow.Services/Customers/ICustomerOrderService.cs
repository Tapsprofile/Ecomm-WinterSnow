namespace WinterSnow.Services.Customers;

public interface ICustomerOrderService
{
    Task<List<CustomerOrderListItem>> GetMyOrdersAsync(int customerId, CancellationToken ct = default);
    Task<CustomerOrderDetailsDto?> GetMyOrderAsync(int customerId, int orderId, CancellationToken ct = default);
}

