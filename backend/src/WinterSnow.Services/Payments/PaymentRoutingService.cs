using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Payments.Providers;
using WinterSnow.Data;

namespace WinterSnow.Services.Payments;

public interface IPaymentRoutingService
{
    Task<PaymentProvider> GetProviderForVendorAsync(int vendorId, CancellationToken ct = default);
    Task<List<PaymentProvider>> ListProvidersAsync(CancellationToken ct = default);
}

public class PaymentRoutingService : IPaymentRoutingService
{
    private readonly WinterSnowDbContext _db;

    public PaymentRoutingService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<List<PaymentProvider>> ListProvidersAsync(CancellationToken ct = default)
        => await _db.PaymentProviders.OrderBy(p => p.DisplayName).ToListAsync(ct);

    public async Task<PaymentProvider> GetProviderForVendorAsync(int vendorId, CancellationToken ct = default)
    {
        // Vendor mapping (priority-based) -> fallback to first active provider.
        var provider = await (from vpp in _db.VendorPaymentProviders
                              join p in _db.PaymentProviders on vpp.PaymentProviderId equals p.Id
                              where vpp.VendorId == vendorId && vpp.IsActive && p.IsActive
                              orderby vpp.Priority, p.Id
                              select p).FirstOrDefaultAsync(ct);

        provider ??= await _db.PaymentProviders.Where(p => p.IsActive).OrderBy(p => p.Id).FirstOrDefaultAsync(ct);

        if (provider is null)
            throw new InvalidOperationException("No active payment provider configured.");

        return provider;
    }
}

