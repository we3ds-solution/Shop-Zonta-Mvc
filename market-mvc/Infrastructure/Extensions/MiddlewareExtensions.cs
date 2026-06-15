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
        /// Configure routing and authentication
        /// </summary>
        public static IApplicationBuilder UseRoutingConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
