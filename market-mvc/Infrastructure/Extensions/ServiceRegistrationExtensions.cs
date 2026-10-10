using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Infrastructure.Data.Seeds;
using market_mvc.Infrastructure.Repositories;
using market_mvc.Domain.Interfaces;
using market_mvc.Features.Home.Services;
using market_mvc.Features.Orders.Services;
using market_mvc.Features.Products.Services;
using market_mvc.Infrastructure.Services;
using FluentValidation;
using System.Reflection;
using market_mvc.Features.Cart.Services;
using market_mvc.Features.Catalog;
using market_mvc.Infrastructure.Localization;

namespace market_mvc.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering application services
    /// Centralizes dependency injection configuration
    /// </summary>
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Register core MVC services
        /// </summary>
        public static IServiceCollection AddCoreMvcServices(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            return services;
        }

        /// <summary>
        /// Register MediatR for CQRS pattern
        /// </summary>
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }

        /// <summary>
        /// Register AutoMapper for object mapping
        /// </summary>
        public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }

        /// <summary>
        /// Register FluentValidation validators
        /// </summary>
        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        /// <summary>
        /// Register application domain services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IZontaLocalizer, ZontaLocalizer>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Feature Services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICatalogImportService, CatalogImportService>();
            services.AddScoped<IProductServ, ProductServ>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<ISearchService, SearchService>();

            // Repositories
            services.AddScoped<IHomeRepository, HomeRepository>();

            return services;
        }

        /// <summary>
        /// Register data seeding services
        /// </summary>
        public static IServiceCollection AddDataSeedingServices(this IServiceCollection services)
        {
            services.AddTransient<SeederOrchestrator>();
            services.AddTransient<ISeeder, ApplicationUserSeeder>();
            services.AddTransient<ISeeder, CategorySeeder>();
            services.AddTransient<ISeeder, CustomerSeeder>();
            services.AddTransient<ISeeder, ProductSeeder>();
            services.AddTransient<ISeeder, OrderSeeder>();
            return services;
        }

        /// <summary>
        /// Register database context
        /// </summary>
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                )
            );
            return services;
        }

        /// <summary>
        /// Register all services in one call
        /// </summary>
        public static IServiceCollection AddAllApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services
                .AddCoreMvcServices()
                .AddMediatRServices()
                .AddAutoMapperServices()
                .AddFluentValidationServices()
                .AddApplicationServices()
                .AddDataSeedingServices()
                .AddDatabaseContext(configuration)
                .AddAuthenticationServices(env)
                .AddAuthorizationPolicies();

            // Session-backed cart (no extra infrastructure).
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(4);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            // Real EN/AR request localization (cookie-driven, culture-aware formatting).
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supported = new[] { "en", "ar" }
                    .Select(c => new System.Globalization.CultureInfo(c)).ToList();
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                options.SupportedCultures = supported;
                options.SupportedUICultures = supported;
                options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider());
            });

            return services;
        }
    }
}
