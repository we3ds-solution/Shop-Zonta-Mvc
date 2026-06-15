using market_mvc.Infrastructure.Common;
using System.Net;
using System.Text.Json;

namespace market_mvc.Infrastructure.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// Catches unhandled exceptions and returns appropriate error responses
    /// Handles custom application exceptions and maps them to HTTP status codes
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {ExceptionMessage}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = notFoundEx.Message,
                        Code = notFoundEx.Code,
                        StatusCode = StatusCodes.Status404NotFound,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case ValidationException validationEx:
                    context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = validationEx.Message,
                        Code = validationEx.Code,
                        StatusCode = StatusCodes.Status422UnprocessableEntity,
                        Errors = validationEx.Failures.SelectMany(x => x.Value).ToList(),
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case UnauthorizedException unauthorizedEx:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = unauthorizedEx.Message,
                        Code = unauthorizedEx.Code,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case ForbiddenException forbiddenEx:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = forbiddenEx.Message,
                        Code = forbiddenEx.Code,
                        StatusCode = StatusCodes.Status403Forbidden,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case ConflictException conflictEx:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = conflictEx.Message,
                        Code = conflictEx.Code,
                        StatusCode = StatusCodes.Status409Conflict,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case BusinessRuleException businessEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = businessEx.Message,
                        Code = businessEx.Code,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                case ApplicationException appEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = appEx.Message,
                        Code = appEx.Code,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Timestamp = DateTime.UtcNow
                    };
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = "An unexpected error occurred. Please try again later.",
                        Code = "INTERNAL_SERVER_ERROR",
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Timestamp = DateTime.UtcNow,
                        TraceId = context.TraceIdentifier
                    };
                    break;
            }

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Standardized error response
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
