using Microsoft.EntityFrameworkCore;
using market_mvc.Domain;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ObjectValues;
using market_mvc.Domain.ViewModels.Cart;

namespace market_mvc.Features.Orders.Services
{
    public record CheckoutInput(
        string UserId,
        string Email,
        string FirstName,
        string LastName,
        string? Phone,
        string Street,
        string City,
        string State,
        string ZipCode,
        string Country,
        string? Notes);

    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct = default);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(string email, CancellationToken ct = default);
        Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request, CancellationToken ct = default);
        Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken ct = default);
        Task<Order?> GetOrderForUserAsync(Guid id, string email, bool isPrivileged, CancellationToken ct = default);
        /// <summary>
        /// Secure checkout: prices/totals/stock validated server-side in a transaction.
        /// Payment is cash-on-delivery (Pending) — no online payment is pretended.
        /// </summary>
        Task<(bool Ok, string? Error, Guid OrderId)> CreateOrderFromCartAsync(CheckoutInput input, CancellationToken ct = default);
        Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default);
        Task DeleteOrderAsync(Guid id, CancellationToken ct = default);

        [Obsolete("Insecure legacy API that trusted client totals. Use CreateOrderFromCartAsync.")]
        Task<Guid> CreateOrderAsync(decimal total);
        Task UpdateOrderAsync(Order order);
    }
}
