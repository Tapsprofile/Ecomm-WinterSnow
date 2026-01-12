using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Payments;
using WinterSnow.Core.Domain.Payments.Providers;
using WinterSnow.Data;

namespace WinterSnow.Services.Payments;

public interface IPaymentAdminService
{
    Task<List<PaymentProvider>> ListProvidersAsync(CancellationToken ct = default);
    Task<int> CreateProviderAsync(string systemName, string displayName, bool isActive, string? configJson, CancellationToken ct = default);
    Task SetProviderActiveAsync(int providerId, bool isActive, CancellationToken ct = default);

    Task SetVendorProviderAsync(int vendorId, int providerId, bool isActive, int priority, CancellationToken ct = default);
    Task<List<VendorPaymentProvider>> ListVendorProvidersAsync(int vendorId, CancellationToken ct = default);

    Task<List<PaymentTransactionDto>> QueryTransactionsAsync(PaymentTransactionQuery query, CancellationToken ct = default);
}

public class PaymentAdminService : IPaymentAdminService
{
    private readonly WinterSnowDbContext _db;

    public PaymentAdminService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public Task<List<PaymentProvider>> ListProvidersAsync(CancellationToken ct = default)
        => _db.PaymentProviders.OrderBy(p => p.DisplayName).ToListAsync(ct);

    public async Task<int> CreateProviderAsync(string systemName, string displayName, bool isActive, string? configJson, CancellationToken ct = default)
    {
        var p = new PaymentProvider
        {
            SystemName = systemName.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            IsActive = isActive,
            ConfigJson = configJson
        };
        _db.PaymentProviders.Add(p);
        await _db.SaveChangesAsync(ct);
        return p.Id;
    }

    public async Task SetProviderActiveAsync(int providerId, bool isActive, CancellationToken ct = default)
    {
        var p = await _db.PaymentProviders.FirstOrDefaultAsync(x => x.Id == providerId, ct);
        if (p is null) return;
        p.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }

    public async Task SetVendorProviderAsync(int vendorId, int providerId, bool isActive, int priority, CancellationToken ct = default)
    {
        var existing = await _db.VendorPaymentProviders.FirstOrDefaultAsync(x => x.VendorId == vendorId && x.PaymentProviderId == providerId, ct);
        if (existing is null)
        {
            _db.VendorPaymentProviders.Add(new VendorPaymentProvider
            {
                VendorId = vendorId,
                PaymentProviderId = providerId,
                IsActive = isActive,
                Priority = priority
            });
        }
        else
        {
            existing.IsActive = isActive;
            existing.Priority = priority;
        }
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<VendorPaymentProvider>> ListVendorProvidersAsync(int vendorId, CancellationToken ct = default)
        => _db.VendorPaymentProviders.Where(x => x.VendorId == vendorId).OrderBy(x => x.Priority).ToListAsync(ct);

    public async Task<List<PaymentTransactionDto>> QueryTransactionsAsync(PaymentTransactionQuery query, CancellationToken ct = default)
    {
        var take = Math.Clamp(query.Take, 1, 500);

        IQueryable<PaymentTransaction> q = _db.PaymentTransactions.AsQueryable();

        if (query.TransactionId is not null) q = q.Where(x => x.Id == query.TransactionId);
        if (query.PaymentGroupId is not null) q = q.Where(x => x.PaymentGroupId == query.PaymentGroupId);
        if (query.VendorId is not null) q = q.Where(x => x.VendorId == query.VendorId);
        if (query.OrderId is not null) q = q.Where(x => x.OrderId == query.OrderId);

        if (query.ProviderId is not null) q = q.Where(x => x.PaymentProviderId == query.ProviderId);
        if (!string.IsNullOrWhiteSpace(query.ProviderSystemName)) q = q.Where(x => x.PaymentProviderSystemName == query.ProviderSystemName);

        if (!string.IsNullOrWhiteSpace(query.ProviderOrderId)) q = q.Where(x => x.ProviderOrderId == query.ProviderOrderId);
        if (!string.IsNullOrWhiteSpace(query.ProviderPaymentId)) q = q.Where(x => x.ProviderPaymentId == query.ProviderPaymentId);
        if (!string.IsNullOrWhiteSpace(query.ProviderPaymentSessionId)) q = q.Where(x => x.ProviderPaymentSessionId == query.ProviderPaymentSessionId);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<PaymentStatus>(query.Status, true, out var st))
            q = q.Where(x => x.Status == st);

        return await q
            .OrderByDescending(x => x.CreatedOnUtc)
            .Take(take)
            .Select(x => new PaymentTransactionDto
            {
                TransactionId = x.Id,
                PaymentGroupId = x.PaymentGroupId,
                OrderId = x.OrderId,
                VendorId = x.VendorId,
                PaymentProviderId = x.PaymentProviderId,
                PaymentProviderSystemName = x.PaymentProviderSystemName,
                PaymentProviderDisplayName = x.PaymentProviderDisplayName,
                Status = x.Status.ToString(),
                ProviderOrderId = x.ProviderOrderId,
                ProviderPaymentId = x.ProviderPaymentId,
                ProviderPaymentSessionId = x.ProviderPaymentSessionId,
                Amount = x.Amount,
                Currency = x.Currency,
                CreatedOnUtc = x.CreatedOnUtc
            })
            .ToListAsync(ct);
    }
}

