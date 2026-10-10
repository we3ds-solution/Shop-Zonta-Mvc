using market_mvc.Domain.ViewModels.Home;
using market_mvc.Domain.ViewModels.Product;
using market_mvc.Domain.Interfaces;

namespace market_mvc.Features.Home.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly ILogger<HomeService> _logger;

        public HomeService(IHomeRepository homeRepository, ILogger<HomeService> logger)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<HomeVM> GetHomeDataAsync()
        {
            try
            {
                _logger.LogInformation("Getting home page data");
                return await _homeRepository.GetHomeDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home data from service");
                return new HomeVM();
            }
        }

        public async Task<List<ProductCardVM>> GetFeaturedProductsAsync(int count = 6)
        {
            try
            {
                _logger.LogInformation("Getting {Count} featured products", count);
                return await _homeRepository.GetFeaturedProductsAsync(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured products from service");
                return new List<ProductCardVM>();
            }
        }

        public async Task<List<ProductCardVM>> GetLatestProductsAsync(int count = 8)
        {
            try
            {
                _logger.LogInformation("Getting {Count} latest products", count);
                return await _homeRepository.GetLatestProductsAsync(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest products from service");
                return new List<ProductCardVM>();
            }
        }

        public async Task<List<ProductCardVM>> GetOnSaleProductsAsync(int count = 6)
        {
            try
            {
                _logger.LogInformation("Getting {Count} on sale products", count);
                return await _homeRepository.GetOnSaleProductsAsync(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting on sale products from service");
                return new List<ProductCardVM>();
            }
        }

        public async Task<Dictionary<string, int>> GetDashboardStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Getting dashboard statistics");
                
                var productStats = await _homeRepository.GetProductStatisticsAsync();
                var totalProducts = await _homeRepository.GetTotalProductsCountAsync();
                var totalOrders = await _homeRepository.GetTotalOrdersCountAsync();
                var totalCustomers = await _homeRepository.GetTotalCustomersCountAsync();
                
                var stats = new Dictionary<string, int>(productStats)
                {
                    ["TotalProducts"] = totalProducts,
                    ["TotalOrders"] = totalOrders,
                    ["TotalCustomers"] = totalCustomers
                };
                
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard statistics from service");
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, decimal>> GetSalesStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Getting sales statistics");
                return await _homeRepository.GetSalesStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales statistics from service");
                return new Dictionary<string, decimal>();
            }
        }

        public async Task<List<ProductCardVM>> GetRecommendedProductsAsync(int count = 4)
        {
            try
            {
                _logger.LogInformation("Getting {Count} recommended products", count);
                
                // For now, return a mix of featured and latest products
                // In the future, this could be enhanced with recommendation algorithms
                var featured = await _homeRepository.GetFeaturedProductsAsync(count / 2);
                var latest = await _homeRepository.GetLatestProductsAsync(count / 2);
                
                var recommended = featured.Concat(latest)
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .Take(count)
                    .ToList();
                
                return recommended;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended products from service");
                return new List<ProductCardVM>();
            }
        }

        public async Task<HomeVM> GetDashboardDataAsync()
        {
            try
            {
                _logger.LogInformation("Getting dashboard data");
                
                var homeData = await GetHomeDataAsync();
                
                // Add additional dashboard-specific data if needed
                // This method can be extended for admin dashboard functionality
                
                return homeData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data from service");
                return new HomeVM();
            }
        }

        public async Task<Dictionary<string, int>> GetProductStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Getting product statistics");
                return await _homeRepository.GetProductStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics from service");
                return new Dictionary<string, int>();
            }
        }
    }
}

