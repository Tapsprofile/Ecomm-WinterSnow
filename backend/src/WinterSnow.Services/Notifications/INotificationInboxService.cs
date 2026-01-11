namespace WinterSnow.Services.Notifications;

public interface INotificationInboxService
{
    Task<int> GetUnreadCountForCustomerAsync(int customerId, CancellationToken ct = default);
    Task<int> GetUnreadCountForVendorAsync(int vendorId, CancellationToken ct = default);

    Task<List<NotificationDto>> GetInboxForCustomerAsync(int customerId, int take = 20, CancellationToken ct = default);
    Task<List<NotificationDto>> GetInboxForVendorAsync(int vendorId, int take = 20, CancellationToken ct = default);

    Task MarkReadAsync(int notificationId, int? customerId, int? vendorId, CancellationToken ct = default);
    Task MarkAllReadAsync(int? customerId, int? vendorId, CancellationToken ct = default);
}

