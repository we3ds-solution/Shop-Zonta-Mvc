using market_mvc.Infrastructure.Common;
using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Queries.GetProducts
{
    public class GetProductsQuery : BaseQuery<PaginatedResult<Product>>
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? PriceRange { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

