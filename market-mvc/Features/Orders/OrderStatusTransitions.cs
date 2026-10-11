using market_mvc.Domain.Enums;

namespace market_mvc.Features.Orders
{
    /// <summary>
    /// Single source of truth for legal order status transitions
    /// (used by OrderService; unit-tested in Zonta.Tests).
    /// </summary>
    public static class OrderStatusTransitions
    {
        public static IReadOnlyList<OrderStatus> AllowedFrom(OrderStatus status) => status switch
        {
            OrderStatus.Pending => new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            OrderStatus.Confirmed => new[] { OrderStatus.Processing, OrderStatus.Cancelled },
            OrderStatus.Processing => new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            OrderStatus.Shipped => new[] { OrderStatus.Delivered },
            _ => Array.Empty<OrderStatus>()
        };
    }
}
