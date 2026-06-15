using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Infrastructure.Repositories;
using market_mvc.Domain.Interfaces;
using market_mvc.Features.Home.Services;
using market_mvc.Features.Orders.Services;
using market_mvc.Features.Products.Services;
using market_mvc.Infrastructure.Services;
using FluentValidation;
using System.Reflection;

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
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Feature Services
            services.AddScoped<IOrderService, OrderService>();
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
        public static IServiceCollection AddAllApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCoreMvcServices()
                .AddMediatRServices()
                .AddAutoMapperServices()
                .AddFluentValidationServices()
                .AddApplicationServices()
                .AddDataSeedingServices()
                .AddDatabaseContext(configuration)
                .AddAuthenticationServices()
                .AddAuthorizationPolicies();

            return services;
        }
    }
}
