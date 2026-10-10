using MediatR;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            if (product == null)
            {
                return false;
            }

            // Soft delete
            product.MarkAsDeleted();
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

