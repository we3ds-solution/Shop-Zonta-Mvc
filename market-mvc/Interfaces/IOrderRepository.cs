using market_mvc.Domain;
using market_mvc.Domain.entity;

namespace market_mvc.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request);
        Task<Order?> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
    }
}


