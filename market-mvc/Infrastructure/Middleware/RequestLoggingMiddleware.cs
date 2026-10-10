namespace market_mvc.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware for logging HTTP requests and responses
    /// Tracks request details for debugging and monitoring
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            _logger.LogInformation("HTTP {Method} {Path} started", request.Method, request.Path);

            var startTime = DateTime.UtcNow;
            
            await _next(context);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("HTTP {Method} {Path} completed with status {StatusCode} in {Duration}ms", 
                request.Method, 
                request.Path, 
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
    }
}
