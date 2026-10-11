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

        public static IServiceCollection AddAuthRateLimiting(this IServiceCollection services, IConfiguration? config = null)
        {
            // Configurable so integration tests can raise the ceiling
            // (Zonta:RateLimit:PermitLimit / WindowMinutes, defaults 5/1).
            // Read per-request (not at startup) so test/host configuration
            // overrides always apply.
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(AuthPolicy, context =>
                {
                    var cfg = context.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration ?? config;
                    var permits = cfg?.GetValue<int?>("Zonta:RateLimit:PermitLimit") is { } p && p > 0 ? p : 5;
                    var minutes = cfg?.GetValue<int?>("Zonta:RateLimit:WindowMinutes") is { } m && m > 0 ? m : 1;
                    var ip = context.Connection.RemoteIpAddress;
                    return RateLimitPartition.GetFixedWindowLimiter(
                        PartitionKey(ip?.ToString()),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = permits,
                            Window = TimeSpan.FromMinutes(minutes),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });
            return services;
        }
    }
}
