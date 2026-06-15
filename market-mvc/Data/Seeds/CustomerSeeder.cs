using Microsoft.EntityFrameworkCore;
using market_mvc.Data;
using market_mvc.Models.entity;
using market_mvc.Models.ObjectValues;

namespace market_mvc.Data.Seeds
{
    /// <summary>
    /// Seeder for customer data
    /// </summary>
    public class CustomerSeeder : ISeeder
    {
        public string Name => "Customers";
        public int Order => 2; // Execute second

        public async Task SeedAsync(DbContext context)
        {
            var appDbContext = context as AppDbContext;
            if (appDbContext == null)
                throw new InvalidOperationException("Context must be AppDbContext");

            if (await appDbContext.Customers.AnyAsync())
            {
                return; // Customers already seeded
            }

            var customers = new List<Customer>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new PersonName("John", "Doe"),
                    Email = "john.doe@example.com",
                    Phone = "+1-555-0123",
                    Address = new Address(
                        street: "123 Main St",
                        city: "New York",
                        state: "NY",
                        zipCode: "10001",
                        country: "USA"
                    ),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new PersonName("Jane", "Smith"),
                    Email = "jane.smith@example.com",
                    Phone = "+1-555-0124",
                    Address = new Address(
                        street: "456 Oak Ave",
                        city: "Los Angeles",
                        state: "CA",
                        zipCode: "90210",
                        country: "USA"
                    ),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new PersonName("Mike", "Johnson"),
                    Email = "mike.johnson@example.com",
                    Phone = "+1-555-0125",
                    Address = new Address(
                        street: "789 Pine Rd",
                        city: "Chicago",
                        state: "IL",
                        zipCode: "60601",
                        country: "USA"
                    ),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                }
            };

            await appDbContext.Customers.AddRangeAsync(customers);
            await appDbContext.SaveChangesAsync();
        }
    }
}

