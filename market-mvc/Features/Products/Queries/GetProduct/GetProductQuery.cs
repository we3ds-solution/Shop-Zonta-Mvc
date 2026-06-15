using MediatR;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Queries.GetProduct
{
    public class GetProductQuery : IRequest<Product?>
    {
        public Guid Id { get; set; }

        public GetProductQuery(Guid id)
        {
            Id = id;
        }
    }
}

