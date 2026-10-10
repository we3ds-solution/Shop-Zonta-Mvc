using market_mvc.Infrastructure.Common;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Specifications
{
    public class ProductWithCategorySpecification : BaseSpecification<Product>
    {
        public ProductWithCategorySpecification() : base()
        {
            AddInclude(p => p.Category);
        }

        public ProductWithCategorySpecification(Guid productId) : base(p => p.Id == productId)
        {
            AddInclude(p => p.Category);
        }
    }
}

