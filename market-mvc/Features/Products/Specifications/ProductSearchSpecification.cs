using market_mvc.Infrastructure.Common;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;

namespace market_mvc.Features.Products.Specifications
{
    public class ProductSearchSpecification : BaseSpecification<Product>
    {
        public ProductSearchSpecification(
            string? searchTerm = null,
            string? category = null,
            ProductStatus? status = null,
            bool? isFeatured = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int skip = 0,
            int take = 10,
            string? sortBy = null,
            bool sortDescending = false)
        {
            // Build criteria expression
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Criteria = p => p.Name.Contains(searchTerm) || 
                               (p.Description != null && p.Description.Contains(searchTerm));
            }

            // Add includes
            AddInclude(p => p.Category);

            // Apply sorting
            switch (sortBy?.ToLower())
            {
                case "name":
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.Name);
                    else
                        ApplyOrderBy(p => p.Name);
                    break;
                case "price":
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.Price);
                    else
                        ApplyOrderBy(p => p.Price);
                    break;
                case "created":
                    if (sortDescending)
                        ApplyOrderByDescending(p => p.CreatedAt);
                    else
                        ApplyOrderBy(p => p.CreatedAt);
                    break;
                default:
                    ApplyOrderByDescending(p => p.CreatedAt);
                    break;
            }

            // Apply paging
            ApplyPaging(skip, take);
        }
    }
}

