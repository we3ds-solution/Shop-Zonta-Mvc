using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ObjectValues;
using market_mvc.Features.Cart.Services;
using market_mvc.Features.Payments;

namespace market_mvc.Features.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        private readonly ICartService _cart;
        private readonly IPaymentProvider _payments;
        private readonly IConfiguration _config;

        public OrderService(IUnitOfWork unitOfWork, AppDbContext db, ICartService cart, IPaymentProvider payments, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _cart = cart;
            _payments = payments;
            _config = config;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct = default)
        {
            return await _unitOfWork.Orders.GetAllAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string email, CancellationToken ct = default)
        {
            var all = await _unitOfWork.Orders.GetAllAsync();
            return all.Where(o => string.Equals(o.CustomerEmail, email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request, CancellationToken ct = default)
        {
            return await _unitOfWork.Orders.GetOrdersAsync(request);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _unitOfWork.Orders.GetByIdAsync(id);
        }

        public async Task<Order?> GetOrderForUserAsync(Guid id, string email, bool isPrivileged, CancellationToken ct = default)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, ct);
            if (order == null) return null;
            if (isPrivileged) return order;
            if (string.Equals(order.CustomerEmail, email, StringComparison.OrdinalIgnoreCase)) return order;
            return null; // do not reveal existence
        }

        public async Task<(bool Ok, string? Error, Guid OrderId)> CreateOrderFromCartAsync(CheckoutInput input, CancellationToken ct = default)
        {
            var cart = await _cart.GetCartAsync(ct);
            if (cart.IsEmpty) return (false, "Your cart is empty.", Guid.Empty);

            if (cart.Items.Any(i => !i.IsAvailable))
                return (false, "Some items in your cart are no longer available.", Guid.Empty);

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
                // Lock-free re-read with authoritative prices + stock (incl. variants).
                var products = await _db.Products
                    .Include(p => p.Variants)
                    .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
                    .ToDictionaryAsync(p => p.Id, ct);

                decimal subTotal = 0m;
                var items = new List<OrderItem>();
                foreach (var line in cart.Items)
                {
                    if (!products.TryGetValue(line.ProductId, out var p))
                        return (false, $"Product '{line.ProductName}' is no longer available.", Guid.Empty);
                    if (p.Status != ProductStatus.Active)
                        return (false, $"Product '{p.Name}' is not available.", Guid.Empty);
                    if (line.Quantity < 1 || line.Quantity > 99)
                        return (false, "Invalid quantity.", Guid.Empty);

                    decimal unitPrice = p.Price;
                    string? variantLabel = null;
                    Guid? variantId = null;
                    string? sku = p.Sku;

                    if (line.VariantId.HasValue)
                    {
                        var v = p.Variants.FirstOrDefault(x => x.Id == line.VariantId.Value && !x.IsDeleted && x.IsActive);
                        if (v == null)
                            return (false, $"Selected size/color for '{p.Name}' is no longer available.", Guid.Empty);
                        if (v.StockQuantity < line.Quantity)
                            return (false, $"Only {v.StockQuantity} unit(s) of '{p.Name} ({v.Label})' available.", Guid.Empty);
                        unitPrice = v.EffectivePrice(p.Price);
                        variantLabel = v.Label;
                        variantId = v.Id;
                        sku = v.Sku ?? p.Sku;
                        v.StockQuantity -= line.Quantity;
                        v.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        if (p.HasVariants)
                            return (false, $"Please select a size/color for '{p.Name}'.", Guid.Empty);
                        if (p.TrackQuantity && p.StockQuantity < line.Quantity)
                            return (false, $"Only {p.StockQuantity} unit(s) of '{p.Name}' available.", Guid.Empty);
                        if (p.TrackQuantity)
                        {
                            p.StockQuantity -= line.Quantity;
                            p.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    subTotal += unitPrice * line.Quantity;
                    items.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = p.Id,
                        ProductVariantId = variantId,
                        VariantLabel = variantLabel,
                        Quantity = line.Quantity,
                        UnitPrice = unitPrice, // authoritative DB price, not client value
                        ProductName = p.Name, // immutable snapshot
                        ProductSku = sku,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                var shipping = Features.Shipping.ZontaShipping.Compute(subTotal, _config).Amount;

                // Link to Customer record when the checkout email matches a registered customer.
                var customer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.Email == input.Email && !c.IsDeleted, ct);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ZON-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                    CustomerId = customer?.Id,
                    CustomerEmail = input.Email,
                    CustomerPhone = input.Phone,
                    CustomerName = new PersonName(input.FirstName, input.LastName),
                    ShippingAddress = new Address(input.Street, input.City, input.State, input.ZipCode, input.Country),
                    SubTotal = subTotal,
                    TaxAmount = 0m,
                    ShippingAmount = shipping,
                    DiscountAmount = 0m,
                    Notes = input.Notes,
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending, // overwritten by provider result below
                    PaymentMethod = _payments.MethodName,
                    CreatedAt = DateTime.UtcNow
                };
                order.CalculateTotal();

                // Charge through the configured provider (COD default: stays Pending).
                // Never mark paid from client input; provider result is authoritative.
                var charge = await _payments.CreateChargeAsync(
                    order.Total, _config["Zonta:Currency"] ?? "EGP", ct);
                if (!charge.Ok)
                    return (false, charge.Error ?? "Payment failed.", Guid.Empty);
                order.PaymentStatus = charge.Status;
                order.PaymentMethod = _payments.MethodName;
                if (charge.TransactionId != null)
                    order.PaymentTransactionId = charge.TransactionId;

                foreach (var item in items) item.OrderId = order.Id;
                order.OrderItems = items;

                _db.Orders.Add(order);
                _db.OrderItems.AddRange(items);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                _cart.Clear(); // prevent duplicate submission on refresh
                return (true, null, order.Id);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, ct)
                ?? throw new KeyNotFoundException("Order not found.");

            // Valid transitions only.
            var allowed = order.Status switch
            {
                OrderStatus.Pending => new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
                OrderStatus.Confirmed => new[] { OrderStatus.Processing, OrderStatus.Cancelled },
                OrderStatus.Processing => new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
                OrderStatus.Shipped => new[] { OrderStatus.Delivered },
                _ => Array.Empty<OrderStatus>()
            };
            if (!allowed.Contains(status))
                throw new InvalidOperationException($"Cannot move order from {order.Status} to {status}.");

            order.Status = status;
            if (status == OrderStatus.Shipped) order.ShippedAt = DateTime.UtcNow;
            if (status == OrderStatus.Delivered) order.DeliveredAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteOrderAsync(Guid id, CancellationToken ct = default)
        {
            await _unitOfWork.Orders.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        [Obsolete("Insecure legacy API that trusted client totals. Use CreateOrderFromCartAsync.")]
        public Task<Guid> CreateOrderAsync(decimal total)
            => throw new NotSupportedException("Client-supplied totals are not accepted. Use checkout.");

        public async Task UpdateOrderAsync(Order order)
        {
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
