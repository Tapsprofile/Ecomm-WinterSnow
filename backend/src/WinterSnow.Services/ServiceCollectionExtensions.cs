using Microsoft.Extensions.DependencyInjection;
using WinterSnow.Data.Repositories;
using WinterSnow.Services.Admin;
using WinterSnow.Services.Catalog;
using WinterSnow.Services.Discovery;
using WinterSnow.Services.Iam;
using WinterSnow.Services.Orders;
using WinterSnow.Services.Payments;
using WinterSnow.Services.System;
using WinterSnow.Services.Vendor;

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

        return services;
    }
}

