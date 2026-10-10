using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// No demo orders: previous seeder created orders with Total=0 and fake
    /// transactions. Real orders come from checkout with server-calculated totals.
    /// Idempotent no-op.
    /// </summary>
    public class OrderSeeder : ISeeder
    {
        public string Name => "Orders";
        public int Order => 4;

        public Task SeedAsync(DbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
