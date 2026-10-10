using market_mvc.Infrastructure.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Services
{
    public class ProductServ : IProductServ
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductServ> _logger;
        
        public ProductServ(IUnitOfWork unitOfWork, ILogger<ProductServ> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<PaginatedResult<Product>> GetProductsAsync(ProductSearchRequest request)
        {
            return await _unitOfWork.Products.GetProductsAsync(request);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<Guid> CreateProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return product.Id;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

