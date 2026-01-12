namespace WinterSnow.Services.Returns;

public interface IReturnService
{
    Task<int> CreateAsync(int customerId, CreateReturnRequest req, CancellationToken ct = default);
    Task<List<ReturnDto>> GetCustomerReturnsAsync(int customerId, CancellationToken ct = default);

    Task<List<ReturnDto>> GetVendorReturnsAsync(int vendorId, CancellationToken ct = default);
    Task VendorApproveAsync(int vendorId, int returnId, bool approve, CancellationToken ct = default);
}

