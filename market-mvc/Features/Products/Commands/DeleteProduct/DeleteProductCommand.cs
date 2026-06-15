using market_mvc.Infrastructure.Common;

namespace market_mvc.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : BaseCommand<bool>
    {
        public Guid Id { get; set; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }
}

