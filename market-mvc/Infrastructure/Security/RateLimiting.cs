using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace market_mvc.Infrastructure.Security
{
    /// <summary>
    /// Brute-force protection for authentication endpoints (issue #4).
    /// Uses the built-in .NET rate limiter (no extra packages).
    /// Partitioned per client IP; unknown IP shares a single bucket (fail-closed).
    /// Behind proxies, configure ForwardedHeaders before this middleware.
    /// </summary>
    public static class RateLimiting
    {
        public const string AuthPolicy = "auth";

        public static string PartitionKey(string? remoteIp)
        {
            if (string.IsNullOrWhiteSpace(remoteIp)) return "unknown";
            return remoteIp.Trim().ToLowerInvariant();
        }

        public static IServiceCollection AddAuthRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(AuthPolicy, context =>
                {
                    var ip = context.Connection.RemoteIpAddress;
                    return RateLimitPartition.GetFixedWindowLimiter(
                        PartitionKey(ip?.ToString()),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });
            return services;
        }
    }
}
