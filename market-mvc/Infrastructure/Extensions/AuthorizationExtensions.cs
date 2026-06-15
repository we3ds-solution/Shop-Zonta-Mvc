using Microsoft.AspNetCore.Authorization;

namespace market_mvc.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering authorization policies
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Register application authorization policies
        /// </summary>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Admin only policies
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                // Admin or Manager policies
                options.AddPolicy("AdminOrManager", policy =>
                    policy.RequireRole("Admin", "Manager"));

                // Support staff and above
                options.AddPolicy("SupportAndAbove", policy =>
                    policy.RequireRole("Admin", "Manager", "Support"));

                // Authenticated users
                options.AddPolicy("Authenticated", policy =>
                    policy.RequireAuthenticatedUser());

                // Customer or higher
                options.AddPolicy("Customer", policy =>
                    policy.RequireRole("Admin", "Manager", "Support", "Customer"));
            });

            return services;
        }
    }
}
