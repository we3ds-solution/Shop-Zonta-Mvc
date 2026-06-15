using market_mvc.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Services
{
    public interface IProductServ
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<PaginatedResult<Product>> GetProductsAsync(ProductSearchRequest request);
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<Guid> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);
    }
}

