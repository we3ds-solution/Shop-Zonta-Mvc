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
using market_mvc.Features.Payments;
using market_mvc.Features.Auth.Email;
using market_mvc.Infrastructure.Localization;
using market_mvc.Infrastructure.Security;

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
            // Payment readiness (#9): COD default; set Payments:Provider=Paymob (plus
            // Paymob:* secrets) to switch without touching checkout code.
            services.AddHttpClient<PaymobProvider>();
            services.AddScoped<IPaymentProvider>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                if (string.Equals(config["Payments:Provider"], "Paymob", StringComparison.OrdinalIgnoreCase))
                    return (IPaymentProvider)sp.GetRequiredService<PaymobProvider>();
                return new CashOnDeliveryProvider();
            });
            services.AddScoped<IProductServ, ProductServ>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<ISearchService, SearchService>();

            // Repositories
            services.AddScoped<IHomeRepository, HomeRepository>();

            // Email delivery (#4): SMTP when configured, dev file-sender otherwise.
            services.AddScoped<IEmailSender>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return string.IsNullOrWhiteSpace(config["Smtp:Host"])
                    ? (IEmailSender)ActivatorUtilities.CreateInstance<FileEmailSender>(sp)
                    : ActivatorUtilities.CreateInstance<SmtpEmailSender>(sp);
            });

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
        /// Register database context. Provider is explicit via Database:Provider
        /// (SqlServer default; Sqlite for hermetic integration tests) so test
        /// overrides never fight the production registration (issue #8).
        /// </summary>
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration.GetValue<string>("Database:Provider");
            var connection = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
            {
                if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
                    options.UseSqlite(connection);
                else
                    options.UseSqlServer(connection);
            });
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
            services.AddAuthRateLimiting(configuration);
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(4);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            // Real EN/AR request localization (cookie-driven, culture-aware formatting).
            services.Configure<RequestLocalizationOptions>(options =>            {
                var supported = new[] { "en", "ar" }
                    .Select(c => new System.Globalization.CultureInfo(c)).ToList();
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                options.SupportedCultures = supported;
                options.SupportedUICultures = supported;
                options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider());
            });

            // IIS/MonsterASP behind Kestrel: trust loopback proxy for real client IPs
            // (fixes rate-limit partition + HTTPS redirect detection behind reverse proxy).
            services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                    | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
                    | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost;
                // Default KnownProxies already includes loopback (IIS on same box).
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.KnownProxies.Add(System.Net.IPAddress.Loopback);
                options.KnownProxies.Add(System.Net.IPAddress.IPv6Loopback);
            });

            // Swagger/OpenAPI (gated by Swagger:Enabled config — off in Production by default).
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "Zonta Egypt API",
                    Version = "v1",
                    Description = "Zonta Egypt — Arabic sneakers & streetwear. COD/Paymob payments, EN/AR localization.",
                    Contact = new Microsoft.OpenApi.OpenApiContact
                    {
                        Name = "Zonta Egypt",
                        Url = new Uri("https://shopzonta.com")
                    }
                });
            });

            // Lightweight health check for uptime monitoring (no external dependencies).
            services.AddHealthChecks();

            return services;
        }
    }
}
