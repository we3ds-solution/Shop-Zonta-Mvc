using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ViewModels.Home;
using market_mvc.Domain.ViewModels.Product;
using market_mvc.Domain.Interfaces;

namespace market_mvc.Infrastructure.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeRepository> _logger;

        public HomeRepository(AppDbContext context, ILogger<HomeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProductCardVM>> GetFeaturedProductsAsync(int count = 6)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsFeatured && p.Status == ProductStatus.Active)
                    .OrderBy(p => p.SortOrder)
                    .ThenByDescending(p => p.CreatedAt)
                    .Take(count)
                    .Select(p => new ProductCardVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CompareAtPrice = p.CompareAtPrice,
                        ImageUrl = p.ImageUrl,
                        ImageAlt = p.ImageAlt,
                        Status = p.Status,
                        IsFeatured = p.IsFeatured,
                        StockQuantity = p.StockQuantity,
                        TrackQuantity = p.TrackQuantity,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured products");
                return new List<ProductCardVM>();
            }
        }

        public async Task<List<ProductCardVM>> GetLatestProductsAsync(int count = 8)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Status == ProductStatus.Active)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(count)
                    .Select(p => new ProductCardVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CompareAtPrice = p.CompareAtPrice,
                        ImageUrl = p.ImageUrl,
                        ImageAlt = p.ImageAlt,
                        Status = p.Status,
                        IsFeatured = p.IsFeatured,
                        StockQuantity = p.StockQuantity,
                        TrackQuantity = p.TrackQuantity,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest products");
                return new List<ProductCardVM>();
            }
        }

        public async Task<List<ProductCardVM>> GetOnSaleProductsAsync(int count = 6)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Status == ProductStatus.Active && 
                               p.CompareAtPrice.HasValue && 
                               p.CompareAtPrice > p.Price)
                    .OrderByDescending(p => (p.CompareAtPrice - p.Price) / p.CompareAtPrice)
                    .Take(count)
                    .Select(p => new ProductCardVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CompareAtPrice = p.CompareAtPrice,
                        ImageUrl = p.ImageUrl,
                        ImageAlt = p.ImageAlt,
                        Status = p.Status,
                        IsFeatured = p.IsFeatured,
                        StockQuantity = p.StockQuantity,
                        TrackQuantity = p.TrackQuantity,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting on sale products");
                return new List<ProductCardVM>();
            }
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            try
            {
                return await _context.Products.CountAsync(p => p.Status == ProductStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total products count");
                return 0;
            }
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            try
            {
                return await _context.Orders.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total orders count");
                return 0;
            }
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            try
            {
                return await _context.Customers.CountAsync(c => c.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total customers count");
                return 0;
            }
        }

        public async Task<int> GetActiveProductsCountAsync()
        {
            try
            {
                return await _context.Products.CountAsync(p => p.Status == ProductStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active products count");
                return 0;
            }
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            try
            {
                return await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending orders count");
                return 0;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .SumAsync(o => o.Total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return 0;
            }
        }

        public async Task<List<ProductCardVM>> GetTopSellingProductsAsync(int count = 5)
        {
            try
            {
                return await _context.OrderItems
                    .Include(oi => oi.Product)
                    .ThenInclude(p => p.Category)
                    .Where(oi => oi.Product.Status == ProductStatus.Active)
                    .GroupBy(oi => oi.Product)
                    .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                    .Take(count)
                    .Select(g => new ProductCardVM
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                        Description = g.Key.Description,
                        Price = g.Key.Price,
                        CompareAtPrice = g.Key.CompareAtPrice,
                        ImageUrl = g.Key.ImageUrl,
                        ImageAlt = g.Key.ImageAlt,
                        Status = g.Key.Status,
                        IsFeatured = g.Key.IsFeatured,
                        StockQuantity = g.Key.StockQuantity,
                        TrackQuantity = g.Key.TrackQuantity,
                        CategoryName = g.Key.Category != null ? g.Key.Category.Name : null
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top selling products");
                return new List<ProductCardVM>();
            }
        }

        public async Task<List<ProductCardVM>> GetLowStockProductsAsync(int count = 10)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Status == ProductStatus.Active && 
                               p.TrackQuantity && 
                               p.LowStockThreshold.HasValue && 
                               p.StockQuantity <= p.LowStockThreshold.Value)
                    .OrderBy(p => p.StockQuantity)
                    .Take(count)
                    .Select(p => new ProductCardVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CompareAtPrice = p.CompareAtPrice,
                        ImageUrl = p.ImageUrl,
                        ImageAlt = p.ImageAlt,
                        Status = p.Status,
                        IsFeatured = p.IsFeatured,
                        StockQuantity = p.StockQuantity,
                        TrackQuantity = p.TrackQuantity,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock products");
                return new List<ProductCardVM>();
            }
        }

        public async Task<HomeVM> GetHomeDataAsync()
        {
            try
            {
                var homeData = new HomeVM
                {
                    FeaturedProducts = await GetFeaturedProductsAsync(),
                    LatestProducts = await GetLatestProductsAsync(),
                    OnSaleProducts = await GetOnSaleProductsAsync(),
                    TotalProducts = await GetTotalProductsCountAsync(),
                    TotalOrders = await GetTotalOrdersCountAsync(),
                    TotalCustomers = await GetTotalCustomersCountAsync()
                };

                return homeData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home data");
                return new HomeVM();
            }
        }

        public async Task<List<ProductCardVM>> GetProductsByStatusAsync(string status, int count = 10)
        {
            try
            {
                var productStatus = Enum.Parse<ProductStatus>(status, true);
                
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Status == productStatus)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(count)
                    .Select(p => new ProductCardVM
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CompareAtPrice = p.CompareAtPrice,
                        ImageUrl = p.ImageUrl,
                        ImageAlt = p.ImageAlt,
                        Status = p.Status,
                        IsFeatured = p.IsFeatured,
                        StockQuantity = p.StockQuantity,
                        TrackQuantity = p.TrackQuantity,
                        CategoryName = p.Category != null ? p.Category.Name : null
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by status: {Status}", status);
                return new List<ProductCardVM>();
            }
        }

        public async Task<Dictionary<string, int>> GetProductStatisticsAsync()
        {
            try
            {
                var stats = new Dictionary<string, int>();
                
                stats["Total"] = await _context.Products.CountAsync();
                stats["Active"] = await _context.Products.CountAsync(p => p.Status == ProductStatus.Active);
                stats["Inactive"] = await _context.Products.CountAsync(p => p.Status == ProductStatus.Inactive);
                stats["OutOfStock"] = await _context.Products.CountAsync(p => p.Status == ProductStatus.OutOfStock);
                stats["Featured"] = await _context.Products.CountAsync(p => p.IsFeatured);
                stats["OnSale"] = await _context.Products.CountAsync(p => p.CompareAtPrice.HasValue && p.CompareAtPrice > p.Price);
                
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                return new Dictionary<string, int>();
            }
        }

        public async Task<Dictionary<string, decimal>> GetSalesStatisticsAsync()
        {
            try
            {
                var stats = new Dictionary<string, decimal>();
                
                // Get total sales value
                var totalSales = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .SumAsync(o => o.Total);
                
                // Get average order value
                var avgOrderValue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .AverageAsync(o => (decimal?)o.Total) ?? 0;
                
                // Get this month's sales
                var thisMonth = DateTime.UtcNow.Date.AddDays(1 - DateTime.UtcNow.Day);
                var thisMonthSales = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= thisMonth)
                    .SumAsync(o => o.Total);
                
                stats["TotalSales"] = totalSales;
                stats["AverageOrderValue"] = avgOrderValue;
                stats["ThisMonthSales"] = thisMonthSales;
                
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales statistics");
                return new Dictionary<string, decimal>();
            }
        }
    }
}

