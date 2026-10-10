using market_mvc.Domain.ViewModels.Home;
using market_mvc.Domain.ViewModels.Product;

namespace market_mvc.Domain.Interfaces
{
    public interface IHomeRepository
    {
        Task<List<ProductCardVM>> GetFeaturedProductsAsync(int count = 6);
        Task<List<ProductCardVM>> GetLatestProductsAsync(int count = 8);
        Task<List<ProductCardVM>> GetOnSaleProductsAsync(int count = 6);
        Task<int> GetTotalProductsCountAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<int> GetTotalCustomersCountAsync();
        Task<int> GetActiveProductsCountAsync();
        Task<int> GetPendingOrdersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<ProductCardVM>> GetTopSellingProductsAsync(int count = 5);
        Task<List<ProductCardVM>> GetLowStockProductsAsync(int count = 10);
        Task<HomeVM> GetHomeDataAsync();
        Task<Dictionary<string, int>> GetProductStatisticsAsync();
        Task<Dictionary<string, decimal>> GetSalesStatisticsAsync();
    }
}
