using MediatR;
using Microsoft.EntityFrameworkCore;
using market_mvc.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResult<Product>>
    {
        private readonly AppDbContext _context;

        public GetProductsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm) || 
                                        (p.Description != null && p.Description.Contains(request.SearchTerm)));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == request.Category);
            }

            // Apply price range filter
            if (!string.IsNullOrEmpty(request.PriceRange))
            {
                var parts = request.PriceRange.Split('-');
                if (parts.Length == 2)
                {
                    if (decimal.TryParse(parts[0], out var minPrice))
                    {
                        query = query.Where(p => p.Price >= minPrice);
                    }
                    
                    if (parts[1] != "+" && decimal.TryParse(parts[1], out var maxPrice))
                    {
                        query = query.Where(p => p.Price <= maxPrice);
                    }
                }
            }

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "price" => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "created" => request.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<Product>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}

