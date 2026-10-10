using Microsoft.AspNetCore.Mvc;
using market_mvc.Domain.ViewModels.Error;
using System.Diagnostics;

namespace market_mvc.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var errorViewModel = new ErrorVM
            {
                StatusCode = statusCode,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                RequestPath = HttpContext.Request.Path,
                HttpMethod = HttpContext.Request.Method,
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                ErrorTime = DateTime.UtcNow
            };

            _logger.LogWarning("HTTP {StatusCode} error occurred for {RequestPath}", statusCode, HttpContext.Request.Path);

            switch (statusCode)
            {
                case 404:
                    errorViewModel.ErrorTitle = "Page Not Found";
                    errorViewModel.ErrorMessage = "The page you are looking for could not be found.";
                    return View("NotFound", errorViewModel);
                case 403:
                    errorViewModel.ErrorTitle = "Access Forbidden";
                    errorViewModel.ErrorMessage = "You don't have permission to access this resource.";
                    return View("Error", errorViewModel);
                case 500:
                    errorViewModel.ErrorTitle = "Internal Server Error";
                    errorViewModel.ErrorMessage = "An internal server error occurred. Please try again later.";
                    return View("Error", errorViewModel);
                default:
                    return View("Error", errorViewModel);
            }
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorVM
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = 500,
                RequestPath = HttpContext.Request.Path,
                HttpMethod = HttpContext.Request.Method,
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                ErrorTime = DateTime.UtcNow,
                ShowDetails = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
            };

            _logger.LogError("Unhandled error occurred for {RequestPath}", HttpContext.Request.Path);

            return View(errorViewModel);
        }

        [Route("Error/Test")]
        public IActionResult TestError()
        {
            // This is for testing error handling in development
            throw new InvalidOperationException("This is a test exception for error handling.");
        }
    }
}

