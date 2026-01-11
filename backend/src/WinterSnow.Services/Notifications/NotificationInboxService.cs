using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Notifications;
using WinterSnow.Data;

namespace WinterSnow.Services.Notifications;

public class NotificationInboxService : INotificationInboxService
{
    private readonly WinterSnowDbContext _db;

    public NotificationInboxService(WinterSnowDbContext db)
    {
        _db = db;
    }

    public Task<int> GetUnreadCountForCustomerAsync(int customerId, CancellationToken ct = default)
        => _db.Notifications.CountAsync(n =>
            n.RecipientType == NotificationRecipientType.Customer &&
            n.RecipientUserId == customerId &&
            !n.IsRead, ct);

    public Task<int> GetUnreadCountForVendorAsync(int vendorId, CancellationToken ct = default)
        => _db.Notifications.CountAsync(n =>
            n.RecipientType == NotificationRecipientType.Vendor &&
            n.RecipientVendorId == vendorId &&
            !n.IsRead, ct);

    public Task<List<NotificationDto>> GetInboxForCustomerAsync(int customerId, int take = 20, CancellationToken ct = default)
        => BaseInboxQuery()
            .Where(n => n.RecipientType == NotificationRecipientType.Customer && n.RecipientUserId == customerId)
            .Take(Math.Clamp(take, 1, 100))
            .Select(ToDto())
            .ToListAsync(ct);

    public Task<List<NotificationDto>> GetInboxForVendorAsync(int vendorId, int take = 20, CancellationToken ct = default)
        => BaseInboxQuery()
            .Where(n => n.RecipientType == NotificationRecipientType.Vendor && n.RecipientVendorId == vendorId)
            .Take(Math.Clamp(take, 1, 100))
            .Select(ToDto())
            .ToListAsync(ct);

    public async Task MarkReadAsync(int notificationId, int? customerId, int? vendorId, CancellationToken ct = default)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId, ct);
        if (n is null) return;

        if (n.RecipientType == NotificationRecipientType.Customer && n.RecipientUserId == customerId)
        {
            n.IsRead = true;
            n.ReadOnUtc = DateTime.UtcNow;
        }
        else if (n.RecipientType == NotificationRecipientType.Vendor && n.RecipientVendorId == vendorId)
        {
            n.IsRead = true;
            n.ReadOnUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkAllReadAsync(int? customerId, int? vendorId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        if (customerId is not null)
        {
            var list = await _db.Notifications
                .Where(n => n.RecipientType == NotificationRecipientType.Customer && n.RecipientUserId == customerId && !n.IsRead)
                .ToListAsync(ct);
            foreach (var n in list)
            {
                n.IsRead = true;
                n.ReadOnUtc = now;
            }
        }
        else if (vendorId is not null)
        {
            var list = await _db.Notifications
                .Where(n => n.RecipientType == NotificationRecipientType.Vendor && n.RecipientVendorId == vendorId && !n.IsRead)
                .ToListAsync(ct);
            foreach (var n in list)
            {
                n.IsRead = true;
                n.ReadOnUtc = now;
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private IQueryable<Notification> BaseInboxQuery()
        => _db.Notifications.OrderByDescending(n => n.CreatedOnUtc);

    private static global::System.Linq.Expressions.Expression<Func<Notification, NotificationDto>> ToDto()
        => n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            ActionUrl = n.ActionUrl,
            IsRead = n.IsRead,
            CreatedOnUtc = n.CreatedOnUtc
        };
}

