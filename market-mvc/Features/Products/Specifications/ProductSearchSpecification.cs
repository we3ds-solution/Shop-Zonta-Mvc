using market_mvc.Domain.entity;
using market_mvc.Infrastructure.Common;

namespace market_mvc.Features.Products.Specifications
{
    /// <summary>
    /// Search specification for products
    /// Encapsulates search, filter, sort, and pagination logic
    /// </summary>
    public class ProductSearchSpecification : SearchSpecification<Product>
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid? CategoryId { get; set; }
        public string? PriceRange { get; set; }

        public ProductSearchSpecification(
            string searchTerm = "",
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Name",
            string orderDirection = "asc",
            decimal? minPrice = null,
            decimal? maxPrice = null,
            Guid? categoryId = null,
            string? priceRange = null)
        {
            SearchTerm = searchTerm;
            PageNumber = pageNumber;
            PageSize = pageSize;
            OrderBy = orderBy;
            OrderDirection = orderDirection;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            CategoryId = categoryId;
            PriceRange = priceRange;

            ValidatePaginationParameters();

            // Build search filter
            BuildSearchFilter();

            // Build price filter
            BuildPriceFilter();

            // Build category filter
            BuildCategoryFilter();

            // Apply ordering
            ApplyOrdering(GetOrderExpression());

            // Apply pagination
            ApplyPaging();

            // Include related entities
            AddInclude(p => p.Category);
        }

        private void BuildSearchFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return;

            var term = SearchTerm.Trim().ToLower();
            ApplySearch(p => 
                p.Name.ToLower().Contains(term) ||
                p.Description.ToLower().Contains(term) ||
                p.Sku.ToLower().Contains(term) ||
                p.Tags.ToLower().Contains(term)
            );
        }

        private void BuildPriceFilter()
        {
            // Parse price range if provided
            if (!string.IsNullOrEmpty(PriceRange))
            {
                ParsePriceRange(PriceRange);
            }

            if (MinPrice.HasValue || MaxPrice.HasValue)
            {
                var hasMin = MinPrice.HasValue;
                var hasMax = MaxPrice.HasValue;

                if (hasMin && hasMax)
                {
                    AddCriteria(p => p.Price >= MinPrice && p.Price <= MaxPrice);
                }
                else if (hasMin)
                {
                    AddCriteria(p => p.Price >= MinPrice);
                }
                else if (hasMax)
                {
                    AddCriteria(p => p.Price <= MaxPrice);
                }
            }
        }

        private void BuildCategoryFilter()
        {
            if (CategoryId.HasValue && CategoryId != Guid.Empty)
            {
                AddCriteria(p => p.CategoryId == CategoryId);
            }
        }

        private void ParsePriceRange(string range)
        {
            range = range.ToLower().Replace("$", "").Replace(",", "");

            if (range == "0-50")
            {
                MinPrice = 0;
                MaxPrice = 50;
            }
            else if (range == "51-100")
            {
                MinPrice = 51;
                MaxPrice = 100;
            }
            else if (range == "101-500")
            {
                MinPrice = 101;
                MaxPrice = 500;
            }
            else if (range == "500+")
            {
                MinPrice = 500;
                MaxPrice = null;
            }
        }

        private Expression<Func<Product, dynamic>> GetOrderExpression()
        {
            return OrderBy?.ToLower() switch
            {
                "price" => p => p.Price,
                "name" => p => p.Name,
                "createdat" => p => p.CreatedAt,
                "popularity" => p => p.SortOrder,
                _ => p => p.Name
            };
        }
    }
}
