using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Core.Domain.Returns;
using WinterSnow.Data;

namespace WinterSnow.Services.Returns;

public class ReturnService : IReturnService
{
    private readonly WinterSnowDbContext _db;

    public ReturnService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(int customerId, CreateReturnRequest req, CancellationToken ct = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == req.OrderId && o.CustomerId == customerId, ct);
        if (order is null)
            throw new InvalidOperationException("Order not found.");

        var item = await _db.OrderItems.FirstOrDefaultAsync(i => i.Id == req.OrderItemId && i.OrderId == order.Id, ct);
        if (item is null)
            throw new InvalidOperationException("Order item not found.");

        var rr = new ReturnRequest
        {
            OrderId = order.Id,
            OrderItemId = item.Id,
            CustomerId = customerId,
            VendorId = order.VendorId,
            Reason = req.Reason.Trim(),
            Notes = req.Notes
        };

        _db.ReturnRequests.Add(rr);
        await _db.SaveChangesAsync(ct);
        return rr.Id;
    }

    public async Task<List<ReturnDto>> GetCustomerReturnsAsync(int customerId, CancellationToken ct = default)
    {
        return await _db.ReturnRequests
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.Id)
            .Select(r => new ReturnDto
            {
                ReturnId = r.Id,
                OrderId = r.OrderId,
                OrderItemId = r.OrderItemId,
                VendorId = r.VendorId,
                Reason = r.Reason,
                Notes = r.Notes,
                Status = r.Status.ToString(),
                ReturnLabelUrl = r.ReturnLabelUrl,
                CreatedOnUtc = r.CreatedOnUtc
            })
            .ToListAsync(ct);
    }

    public async Task<List<ReturnDto>> GetVendorReturnsAsync(int vendorId, CancellationToken ct = default)
    {
        return await _db.ReturnRequests
            .Where(r => r.VendorId == vendorId)
            .OrderByDescending(r => r.Id)
            .Select(r => new ReturnDto
            {
                ReturnId = r.Id,
                OrderId = r.OrderId,
                OrderItemId = r.OrderItemId,
                VendorId = r.VendorId,
                Reason = r.Reason,
                Notes = r.Notes,
                Status = r.Status.ToString(),
                ReturnLabelUrl = r.ReturnLabelUrl,
                CreatedOnUtc = r.CreatedOnUtc
            })
            .ToListAsync(ct);
    }

    public async Task VendorApproveAsync(int vendorId, int returnId, bool approve, CancellationToken ct = default)
    {
        var rr = await _db.ReturnRequests.FirstOrDefaultAsync(r => r.Id == returnId && r.VendorId == vendorId, ct);
        if (rr is null)
            throw new InvalidOperationException("Return request not found.");

        rr.Status = approve ? ReturnStatus.VendorApproved : ReturnStatus.VendorRejected;

        // Label generation is a placeholder; integrate logistics provider later.
        if (approve)
            rr.ReturnLabelUrl = rr.ReturnLabelUrl ?? $"https://example.local/labels/return-{rr.Id}.pdf";

        await _db.SaveChangesAsync(ct);
    }
}

