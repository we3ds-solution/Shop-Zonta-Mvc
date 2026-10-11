using market_mvc.Infrastructure.Middleware;

namespace market_mvc.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering middleware in the application pipeline
    /// Centralizes all middleware configuration
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Configure the HTTP request pipeline for development environment
        /// </summary>
        public static IApplicationBuilder UseDevelopmentPipeline(this IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            return app;
        }

        /// <summary>
        /// Configure the HTTP request pipeline for production environment
        /// </summary>
        public static IApplicationBuilder UseProductionPipeline(this IApplicationBuilder app)
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            return app;
        }

        /// <summary>
        /// Register custom middleware components
        /// </summary>
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            return app;
        }

        /// <summary>
        /// Configure status code pages and redirects
        /// </summary>
        public static IApplicationBuilder UseStatusCodeConfiguration(this IApplicationBuilder app)
        {
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            return app;
        }

        /// <summary>
        /// Configure security and HTTP settings
        /// </summary>
        public static IApplicationBuilder UseSecurityConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            return app;
        }

        /// <summary>
        /// Trust IIS/X-Forwarded headers (MonsterASP reverse proxy) for correct
        /// client IPs + HTTPS detection. Must run FIRST in the pipeline.
        /// </summary>
        public static IApplicationBuilder UseForwardedHeadersConfiguration(this IApplicationBuilder app)
        {
            var opts = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>>();
            app.UseForwardedHeaders(opts.Value);
            return app;
        }

        /// <summary>
        /// Swagger UI + JSON endpoint. Gated by Swagger:Enabled (bool).
        /// Off in Production by default; enabled in Development or when config says true.
        /// </summary>
        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IConfiguration config)
        {
            if (!config.GetValue<bool>("Swagger:Enabled")) return app;

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Zonta Egypt API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "Zonta Egypt — API Docs";
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
            });
            return app;
        }

        /// <summary>
        /// Health check endpoint for uptime monitoring (MonsterASP/load balancer).
        /// </summary>
        public static IEndpointRouteBuilder MapHealthEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health");
            return endpoints;
        }

        /// <summary>
        /// Configure routing and authentication.
        /// UseRateLimiter MUST run after UseRouting so endpoint metadata
        /// ([EnableRateLimiting]) is visible; before it, policies never apply.
        /// </summary>
        public static IApplicationBuilder UseRoutingConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
