using MediatR;
using market_mvc.Data;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetProductQueryHandler> _logger;

        public GetProductQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetProductQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Product?> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", request.Id);

            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", request.Id);
            }

            return product;
        }
    }
}

