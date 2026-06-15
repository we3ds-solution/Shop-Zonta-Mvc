using market_mvc.Infrastructure.Common;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : BaseQuery<Product?>
    {
        public Guid Id { get; set; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

