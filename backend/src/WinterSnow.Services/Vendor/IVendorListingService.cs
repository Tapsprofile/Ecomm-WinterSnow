namespace WinterSnow.Services.Vendor;

public interface IVendorListingService
{
    Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<int> CreateListingAsync(int vendorId, CreateListingRequest request, CancellationToken ct = default);
    Task UpdateListingAsync(int vendorId, int productId, UpdateListingRequest request, CancellationToken ct = default);
}

