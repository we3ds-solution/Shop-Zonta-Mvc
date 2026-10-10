using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// No demo customers in any environment: real customers come from registration.
    /// Previous fake US customers (John Doe etc.) are not seeded. Idempotent no-op.
    /// </summary>
    public class CustomerSeeder : ISeeder
    {
        public string Name => "Customers";
        public int Order => 2;

        public Task SeedAsync(DbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
