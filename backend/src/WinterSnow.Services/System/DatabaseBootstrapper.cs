using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Catalog;
using WinterSnow.Core.Domain.Customers;
using WinterSnow.Core.Domain.Payments.Providers;
using WinterSnow.Core.Domain.Reviews;
using WinterSnow.Data;
using WinterSnow.Services.Iam;
using VendorEntity = WinterSnow.Core.Domain.Customers.Vendor;

namespace WinterSnow.Services.System;

public class DatabaseBootstrapper
{
    private readonly WinterSnowDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseBootstrapper(WinterSnowDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task EnsureSeededAsync(CancellationToken ct = default)
    {
        await _db.Database.EnsureCreatedAsync(ct);

        if (await _db.Vendors.AnyAsync(ct))
            return;

        var vendor1 = new VendorEntity
        {
            Name = "Winter Snow Official",
            DisplayName = "Winter Snow",
            IsActive = true,
            IsKycApproved = true,
            CommissionRate = 0.15m
        };
        var vendor2 = new VendorEntity
        {
            Name = "SnowPeak Gear",
            DisplayName = "SnowPeak",
            IsActive = false,
            IsKycApproved = false,
            CommissionRate = 0.12m
        };

        _db.Vendors.AddRange(vendor1, vendor2);
        await _db.SaveChangesAsync(ct);

        // Payment providers (multi-gateway)
        var cashfree = new PaymentProvider { SystemName = "cashfree", DisplayName = "Cashfree", IsActive = true };
        var razorpay = new PaymentProvider { SystemName = "razorpay", DisplayName = "Razorpay", IsActive = true };
        _db.PaymentProviders.AddRange(cashfree, razorpay);
        await _db.SaveChangesAsync(ct);

        // Vendor routing: vendor1 -> cashfree, vendor2 -> razorpay (demo)
        _db.VendorPaymentProviders.AddRange(
            new VendorPaymentProvider { VendorId = vendor1.Id, PaymentProviderId = cashfree.Id, IsActive = true, Priority = 0 },
            new VendorPaymentProvider { VendorId = vendor2.Id, PaymentProviderId = razorpay.Id, IsActive = true, Priority = 0 }
        );
        await _db.SaveChangesAsync(ct);

        var customerUser = new AppUser
        {
            Email = "customer@demo.local",
            PasswordHash = _passwordHasher.HashPassword("Customer123!"),
            UserType = UserType.Customer,
            IsEmailVerified = true,
            IsMfaEnabled = false
        };

        _db.Users.AddRange(
            new AppUser
            {
                Email = "admin@demo.local",
                PasswordHash = _passwordHasher.HashPassword("Admin123!"),
                UserType = UserType.Admin,
                IsEmailVerified = true,
                IsMfaEnabled = false
            },
            new AppUser
            {
                Email = "vendor@demo.local",
                PasswordHash = _passwordHasher.HashPassword("Vendor123!"),
                UserType = UserType.Vendor,
                VendorId = vendor1.Id,
                IsEmailVerified = true,
                IsMfaEnabled = false
            },
            customerUser
        );
        await _db.SaveChangesAsync(ct);

        var customerId = customerUser.Id;

        var p1 = new Product
        {
            Name = "Senior Software Developer",
            Slug = "senior-software-developer",
            ShortDescription = "Demo product listing (service/role).",
            FullDescription = "A starter product seeded for the WinterSnow marketplace. Replace this with real SKUs and rich descriptions.",
            Material = "N/A",
            Price = 1999,
            Currency = "INR",
            VendorId = vendor1.Id,
            AllowCoupons = true,
            DiscountPercent = 10,
            DiscountStartUtc = DateTime.UtcNow.AddDays(-1),
            DiscountEndUtc = DateTime.UtcNow.AddDays(7),
            ListingStatus = WinterSnow.Core.Domain.Catalog.Listings.ListingStatus.Active,
            IsVisibleInStorefront = true,
            Published = true,
            IsApprovedByAdmin = true,
            MetaTitle = "Senior Software Developer",
            MetaDescription = "Demo seeded product: Senior Software Developer"
        };

        var p2 = new Product
        {
            Name = "Winter Collection: Thermal Gloves",
            Slug = "thermal-gloves",
            ShortDescription = "Windproof thermal gloves for snow rides.",
            FullDescription = "Warm inner lining, grippy palm, and touch support.",
            Material = "Fleece",
            Price = 799,
            Currency = "INR",
            VendorId = vendor1.Id,
            AllowCoupons = true,
            ListingStatus = WinterSnow.Core.Domain.Catalog.Listings.ListingStatus.Active,
            IsVisibleInStorefront = true,
            Published = true,
            IsApprovedByAdmin = true
        };

        var p3 = new Product
        {
            Name = "Winter Collection: Snow Jacket Pro",
            Slug = "snow-jacket-pro",
            ShortDescription = "Waterproof snow jacket with hood.",
            FullDescription = "Premium shell with breathable membrane.",
            Material = "Polyester",
            Price = 3499,
            Currency = "INR",
            VendorId = vendor2.Id,
            AllowCoupons = false,
            ListingStatus = WinterSnow.Core.Domain.Catalog.Listings.ListingStatus.Draft,
            IsVisibleInStorefront = false,
            Published = false,
            IsApprovedByAdmin = false
        };

        _db.Products.AddRange(p1, p2, p3);
        await _db.SaveChangesAsync(ct);

        _db.ProductMedia.AddRange(
            new ProductMedia { ProductId = p1.Id, Url = "https://picsum.photos/seed/senior-dev/1200/800", DisplayOrder = 1 },
            new ProductMedia { ProductId = p1.Id, Url = "https://picsum.photos/seed/senior-dev-2/1200/800", DisplayOrder = 2 },
            new ProductMedia { ProductId = p2.Id, Url = "https://picsum.photos/seed/gloves/1200/800", DisplayOrder = 1 },
            new ProductMedia { ProductId = p3.Id, Url = "https://picsum.photos/seed/jacket/1200/800", DisplayOrder = 1 }
        );

        _db.ProductVariants.AddRange(
            new ProductVariant { ProductId = p1.Id, Size = "Standard", Color = "Black", Sku = "SSD-STD-BLK", StockQuantity = 999 },
            new ProductVariant { ProductId = p2.Id, Size = "S", Color = "Black", Sku = "GLV-S-BLK", StockQuantity = 50 },
            new ProductVariant { ProductId = p2.Id, Size = "M", Color = "Black", Sku = "GLV-M-BLK", StockQuantity = 40 },
            new ProductVariant { ProductId = p2.Id, Size = "L", Color = "Black", Sku = "GLV-L-BLK", StockQuantity = 30 },
            new ProductVariant { ProductId = p3.Id, Size = "M", Color = "Blue", Sku = "JKT-M-BLU", StockQuantity = 25 }
        );

        _db.Reviews.AddRange(
            new Review
            {
                ProductId = p1.Id,
                CustomerId = customerId,
                Rating = 5,
                Title = "Great listing",
                ReviewText = "Using this seeded product to validate UGC + review flows.",
                IsVerifiedPurchase = false
            },
            new Review
            {
                ProductId = p2.Id,
                CustomerId = customerId,
                Rating = 4,
                Title = "Warm and comfy",
                ReviewText = "Gloves are great for early morning rides.",
                IsVerifiedPurchase = true
            }
        );

        await _db.SaveChangesAsync(ct);

        // Demo notification seed
        _db.Notifications.Add(new WinterSnow.Core.Domain.Notifications.Notification
        {
            RecipientType = WinterSnow.Core.Domain.Notifications.NotificationRecipientType.Customer,
            RecipientUserId = customerId,
            Title = "Welcome to WinterSnow",
            Body = "Your in-app notifications will appear in the top-right bell.",
            ActionUrl = "/"
        });
        await _db.SaveChangesAsync(ct);
    }
}

