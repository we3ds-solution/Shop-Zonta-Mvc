using AutoMapper;
using MediatR;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            
            // Generate SKU if not provided
            if (string.IsNullOrEmpty(product.Sku))
            {
                product.Sku = $"PRD-{DateTime.UtcNow:yyyyMMdd}-{product.Id.ToString()[..8].ToUpper()}";
            }

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return product;
        }
    }
}

