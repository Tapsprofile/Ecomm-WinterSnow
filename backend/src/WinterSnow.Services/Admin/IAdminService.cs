namespace WinterSnow.Services.Admin;

public interface IAdminService
{
    Task<List<PendingVendor>> GetPendingVendorsAsync(CancellationToken ct = default);
    Task<List<PendingProduct>> GetPendingProductsAsync(CancellationToken ct = default);

    Task ApproveVendorKycAsync(int vendorId, CancellationToken ct = default);
    Task ApproveProductAsync(int productId, CancellationToken ct = default);

    Task<FinanceSummary> GetFinanceSummaryAsync(CancellationToken ct = default);
}

