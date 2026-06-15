using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;
using market_mvc.Domain.Interfaces;

namespace market_mvc.Infrastructure.Repositories
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _context;
        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<PaginatedResult<Product>> GetProductsAsync(ProductSearchRequest request)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(p => p.Name.Contains(request.Search));
            }

            // Apply price filters
            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }

            // Apply price range filter
            if (!string.IsNullOrEmpty(request.PriceRange))
            {
                switch (request.PriceRange.ToLower())
                {
                    case "0-50":
                        query = query.Where(p => p.Price >= 0 && p.Price <= 50);
                        break;
                    case "51-100":
                        query = query.Where(p => p.Price >= 51 && p.Price <= 100);
                        break;
                    case "101-500":
                        query = query.Where(p => p.Price >= 101 && p.Price <= 500);
                        break;
                    case "500+":
                        query = query.Where(p => p.Price > 500);
                        break;
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "name":
                        query = request.SortDirection.ToLower() == "desc" 
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                    case "price":
                        query = request.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;
                    default:
                        query = query.OrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            // Get total count
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResult<Product>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            await _context.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            _context.Products.Update(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public void Delete(Product product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            _context.Products.Remove(product);
        }
    }
}

