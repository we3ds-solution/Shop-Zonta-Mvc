using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ObjectValues;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// Seeder for order and order item data
    /// </summary>
    public class OrderSeeder : ISeeder
    {
        public string Name => "Orders";
        public int Order => 4; // Execute last (after customers and products)

        public async Task SeedAsync(DbContext context)
        {
            var appDbContext = context as AppDbContext;
            if (appDbContext == null)
                throw new InvalidOperationException("Context must be AppDbContext");

            if (await appDbContext.Orders.AnyAsync())
            {
                return; // Orders already seeded
            }

            var customers = await appDbContext.Customers.ToListAsync();
            var products = await appDbContext.Products.ToListAsync();

            if (!customers.Any() || !products.Any())
            {
                return; // Cannot seed orders without customers and products
            }

            var orders = new List<Order>();
            var orderItems = new List<OrderItem>();

            // Order 1: Delivered Order
            var order1 = CreateOrder(
                orderNumber: "ORD-2024-001",
                customerId: customers[0].Id,
                customerEmail: customers[0].Email,
                customerPhone: customers[0].Phone,
                customerFirstName: customers[0].Name.FirstName,
                customerLastName: customers[0].Name.LastName,
                shippingAddress: customers[0].Address,
                status: OrderStatus.Delivered,
                paymentStatus: PaymentStatus.Paid,
                paymentMethod: "Credit Card",
                paymentTransactionId: "TXN-001-2024",
                trackingNumber: "TRK123456789",
                createdDaysAgo: 7,
                shippedDaysAgo: 5,
                deliveredDaysAgo: 2
            );
            orders.Add(order1);

            // Order 1 Items
            orderItems.AddRange(new[]
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order1.Id,
                    ProductId = products[0].Id, // Headphones
                    Quantity = 1,
                    UnitPrice = 199.99m,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order1.Id,
                    ProductId = products[1].Id, // T-Shirt
                    Quantity = 2,
                    UnitPrice = 24.99m,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                }
            });

            // Order 2: Processing Order
            var order2 = CreateOrder(
                orderNumber: "ORD-2024-002",
                customerId: customers[1].Id,
                customerEmail: customers[1].Email,
                customerPhone: customers[1].Phone,
                customerFirstName: customers[1].Name.FirstName,
                customerLastName: customers[1].Name.LastName,
                shippingAddress: customers[1].Address,
                status: OrderStatus.Processing,
                paymentStatus: PaymentStatus.Paid,
                paymentMethod: "PayPal",
                paymentTransactionId: "TXN-002-2024",
                trackingNumber: null,
                createdDaysAgo: 3,
                shippedDaysAgo: null,
                deliveredDaysAgo: null
            );
            orders.Add(order2);

            // Order 2 Items
            orderItems.AddRange(new[]
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order2.Id,
                    ProductId = products[2].Id, // Book
                    Quantity = 1,
                    UnitPrice = 49.99m,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order2.Id,
                    ProductId = products[3].Id, // Security Camera
                    Quantity = 1,
                    UnitPrice = 129.99m,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                }
            });

            await appDbContext.Orders.AddRangeAsync(orders);
            await appDbContext.OrderItems.AddRangeAsync(orderItems);
            await appDbContext.SaveChangesAsync();
        }

        private static Order CreateOrder(
            string orderNumber,
            Guid customerId,
            string customerEmail,
            string customerPhone,
            string customerFirstName,
            string customerLastName,
            Address shippingAddress,
            OrderStatus status,
            PaymentStatus paymentStatus,
            string paymentMethod,
            string paymentTransactionId,
            string? trackingNumber,
            int createdDaysAgo,
            int? shippedDaysAgo,
            int? deliveredDaysAgo)
        {
            var now = DateTime.UtcNow;
            var createdAt = now.AddDays(-createdDaysAgo);

            return new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                CustomerId = customerId,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                CustomerName = new PersonName(customerFirstName, customerLastName),
                ShippingAddress = new Address(
                    street: shippingAddress?.Street ?? "Unknown",
                    city: shippingAddress?.City ?? "Unknown",
                    state: shippingAddress?.State ?? "Unknown",
                    zipCode: shippingAddress?.ZipCode ?? "00000",
                    country: shippingAddress?.Country ?? "Unknown"
                ),
                SubTotal = 0m, // Will be calculated by application logic
                TaxAmount = 0m,
                ShippingAmount = 9.99m,
                DiscountAmount = 0m,
                Total = 0m,
                Status = status,
                PaymentStatus = paymentStatus,
                PaymentMethod = paymentMethod,
                PaymentTransactionId = paymentTransactionId,
                TrackingNumber = trackingNumber,
                CreatedAt = createdAt,
                ShippedAt = shippedDaysAgo.HasValue ? now.AddDays(-shippedDaysAgo.Value) : null,
                DeliveredAt = deliveredDaysAgo.HasValue ? now.AddDays(-deliveredDaysAgo.Value) : null,
                CreatedBy = "Seeder"
            };
        }
    }
}


