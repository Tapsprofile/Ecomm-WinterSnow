namespace WinterSnow.Services.Vendor;

public interface IVendorDashboardService
{
    Task<VendorSalesOverview> GetSalesOverviewAsync(int vendorId, CancellationToken ct = default);
    Task<List<VendorInventoryItem>> GetInventoryAsync(int vendorId, CancellationToken ct = default);
    Task BulkUpdateInventoryAsync(int vendorId, BulkUpdateInventoryRequest request, CancellationToken ct = default);
}

