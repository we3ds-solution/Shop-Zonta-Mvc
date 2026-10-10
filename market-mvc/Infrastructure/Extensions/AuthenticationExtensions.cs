using Microsoft.AspNetCore.Identity;
using market_mvc.Domain.entity;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering ASP.NET Core Identity services
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Register ASP.NET Core Identity with ApplicationUser and IdentityRole
        /// </summary>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password requirements
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        /// <summary>
        /// Configure cookie authentication and default schemes
        /// </summary>
        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, IWebHostEnvironment? env = null)
        {
            services.ConfigureApplicationCookie(options =>
            {
                // Actual AuthController routes are /Auth/* (legacy /Account/* paths caused redirect loops).
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                // Strict in production (HTTPS), lax for local HTTP dev so login still works.
                var isProd = env?.IsProduction() ?? true;
                options.Cookie.SecurePolicy = isProd
                    ? CookieSecurePolicy.Always
                    : CookieSecurePolicy.SameAsRequest;
            });

            return services;
        }

        /// <summary>
        /// Register all authentication and authorization services
        /// </summary>
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IWebHostEnvironment? env = null)
        {
            services
                .AddIdentityServices()
                .AddCookieAuthentication(env);

            return services;
        }
    }
}
