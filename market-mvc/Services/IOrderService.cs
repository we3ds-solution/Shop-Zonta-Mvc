using market_mvc.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request);
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<Guid> CreateOrderAsync(decimal total);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Guid id);
    }
}

