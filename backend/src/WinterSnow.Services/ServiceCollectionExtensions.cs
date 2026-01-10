using Microsoft.Extensions.DependencyInjection;
using WinterSnow.Data.Repositories;
using WinterSnow.Services.Admin;
using WinterSnow.Services.Auditing;
using WinterSnow.Services.Catalog;
using WinterSnow.Services.Configuration;
using WinterSnow.Services.Discovery;
using WinterSnow.Services.Iam;
using WinterSnow.Services.Orders;
using WinterSnow.Services.Payments;
using WinterSnow.Services.Returns;
using WinterSnow.Services.System;
using WinterSnow.Services.Vendor;
using WinterSnow.Services.Webhooks;

namespace WinterSnow.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers service layer (nopCommerce-style "Services" project).
    /// Data registrations (DbContext etc.) should be done by the host.
    /// </summary>
    public static IServiceCollection AddWinterSnowServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddSingleton<ApiMetrics>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IProductCatalogService, ProductCatalogService>();
        services.AddScoped<ISearchService, SearchService>();

        services.AddScoped<IAddressValidationService, AddressValidationService>();
        services.AddScoped<ICashfreeGateway, FakeCashfreeGateway>();
        services.AddScoped<ICheckoutService, CheckoutService>();

        services.AddScoped<IVendorDashboardService, VendorDashboardService>();
        services.AddScoped<IVendorListingService, VendorListingService>();
        services.AddScoped<IAdminService, AdminService>();

        // Shopify parity foundations
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IWebhookAdminService, WebhookAdminService>();
        services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();
        services.AddScoped<IReturnService, ReturnService>();

        return services;
    }
}

