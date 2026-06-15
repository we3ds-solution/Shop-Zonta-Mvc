namespace market_mvc.Infrastructure.Common
{
    /// <summary>
    /// Generic result wrapper for all API/Service responses
    /// Standardizes success/failure handling across the application
    /// </summary>
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public Dictionary<string, string[]> ValidationErrors { get; set; } = new();

        // Status codes for HTTP responses
        public int StatusCode { get; set; } = 200;

        public static Result<T> SuccessResult(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static Result<T> FailureResult(string message, int statusCode = 400)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = new List<string> { message }
            };
        }

        public static Result<T> FailureResult(List<string> errors, int statusCode = 400)
        {
            return new Result<T>
            {
                Success = false,
                Message = "Operation failed",
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static Result<T> ValidationFailure(Dictionary<string, string[]> errors, int statusCode = 422)
        {
            return new Result<T>
            {
                Success = false,
                Message = "Validation failed",
                StatusCode = statusCode,
                ValidationErrors = errors
            };
        }

        public static Result<T> NotFound(string message = "Resource not found")
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = 404,
                Errors = new List<string> { message }
            };
        }

        public static Result<T> Unauthorized(string message = "Unauthorized access")
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = 401,
                Errors = new List<string> { message }
            };
        }

        public static Result<T> Forbidden(string message = "Access denied")
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                StatusCode = 403,
                Errors = new List<string> { message }
            };
        }
    }

    /// <summary>
    /// Non-generic result for void operations
    /// </summary>
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; } = 200;

        public static Result SuccessResult(string message = "Operation successful", int statusCode = 200)
        {
            return new Result
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static Result FailureResult(string message, int statusCode = 400)
        {
            return new Result
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = new List<string> { message }
            };
        }

        public static Result FailureResult(List<string> errors, int statusCode = 400)
        {
            return new Result
            {
                Success = false,
                Message = "Operation failed",
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static Result NotFound(string message = "Resource not found")
        {
            return new Result
            {
                Success = false,
                Message = message,
                StatusCode = 404,
                Errors = new List<string> { message }
            };
        }
    }
}
