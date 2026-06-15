using market_mvc.Infrastructure.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Features.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Orders.GetAllAsync();
        }

        public async Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request)
        {
            return await _unitOfWork.Orders.GetOrdersAsync(request);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _unitOfWork.Orders.GetByIdAsync(id);
        }

        public async Task<Guid> CreateOrderAsync(decimal total)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Total = total,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return order.Id;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            await _unitOfWork.Orders.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

