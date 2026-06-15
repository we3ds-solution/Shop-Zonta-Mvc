using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using market_mvc.Domain.ViewModels.Error;
using market_mvc.Domain.ViewModels.Home;
using market_mvc.Features.Home.Services;

namespace market_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await _homeService.GetHomeDataAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading home page");
                return View(new HomeVM()); // Return empty view model on error
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorVM = new ErrorVM 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = HttpContext.Response.StatusCode,
                RequestPath = HttpContext.Request.Path,
                HttpMethod = HttpContext.Request.Method,
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };
            
            return View(errorVM);
        }
    }
}

