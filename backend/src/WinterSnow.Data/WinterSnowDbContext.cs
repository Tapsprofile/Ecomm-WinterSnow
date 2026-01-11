using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Customers;
using WinterSnow.Core.Domain.Orders;
using WinterSnow.Core.Domain.Payments;
using WinterSnow.Core.Domain.Payments.Providers;
using WinterSnow.Core.Domain.Reviews;
using WinterSnow.Core.Domain.Marketing;
using WinterSnow.Core.Domain.Configuration;
using WinterSnow.Core.Domain.Auditing;
using WinterSnow.Core.Domain.Webhooks;
using WinterSnow.Core.Domain.Returns;
using WinterSnow.Core.Domain.Notifications;

namespace WinterSnow.Data;

public class WinterSnowDbContext : DbContext
{
    public WinterSnowDbContext(DbContextOptions<WinterSnowDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductMedia> ProductMedia => Set<ProductMedia>();

    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewMedia> ReviewMedia => Set<ReviewMedia>();

    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentProvider> PaymentProviders => Set<PaymentProvider>();
    public DbSet<VendorPaymentProvider> VendorPaymentProviders => Set<VendorPaymentProvider>();
    public DbSet<VendorLedgerEntry> VendorLedgerEntries => Set<VendorLedgerEntry>();
    public DbSet<VendorPayout> VendorPayouts => Set<VendorPayout>();

    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<FlashSaleEvent> FlashSaleEvents => Set<FlashSaleEvent>();

    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(b =>
        {
            b.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Setting>(b =>
        {
            b.HasIndex(x => x.Key).IsUnique();
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.VendorId);
            b.HasIndex(x => x.CategoryId);
        });

        modelBuilder.Entity<ProductVariant>(b =>
        {
            b.HasIndex(x => new { x.ProductId, x.Sku });
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.VendorId);
            b.HasIndex(x => x.CreatedOnUtc);
        });

        modelBuilder.Entity<PaymentProvider>(b =>
        {
            b.HasIndex(x => x.SystemName).IsUnique();
        });

        modelBuilder.Entity<VendorPaymentProvider>(b =>
        {
            b.HasIndex(x => new { x.VendorId, x.PaymentProviderId }).IsUnique();
        });

        modelBuilder.Entity<PaymentTransaction>(b =>
        {
            b.HasIndex(x => x.PaymentGroupId);
            b.HasIndex(x => x.OrderId);
            b.HasIndex(x => x.VendorId);
            b.HasIndex(x => x.PaymentProviderId);
            b.HasIndex(x => x.PaymentProviderSystemName);
            b.HasIndex(x => x.ProviderOrderId);
            b.HasIndex(x => x.ProviderPaymentId);
        });

        modelBuilder.Entity<Notification>(b =>
        {
            b.HasIndex(x => x.CreatedOnUtc);
            b.HasIndex(x => x.IsRead);
            b.HasIndex(x => new { x.RecipientType, x.RecipientUserId });
            b.HasIndex(x => new { x.RecipientType, x.RecipientVendorId });
        });
    }
}

