using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// Intentionally seeds NO products. Official shopzonta.com catalog could not be
    /// verified (Cloudflare 409 on 2026-10-10), and fabricating products/prices is
    /// prohibited. Use docs/ZONTA-CATALOG.md + Infrastructure/Data/Catalog/ZontaCatalog.json
    /// import workflow once data is verified. Existing demo products (WBH-001 etc.)
    /// are left untouched here; see README for manual cleanup.
    /// </summary>
    public class ProductSeeder : ISeeder
    {
        private readonly ILogger<ProductSeeder> _logger;

        public string Name => "Products";
        public int Order => 3;

        public ProductSeeder(ILogger<ProductSeeder> logger)
        {
            _logger = logger;
        }

        public Task SeedAsync(DbContext context)
        {
            _logger.LogInformation("ProductSeeder: no automatic products seeded. Import verified Zonta catalog manually (see docs/ZONTA-CATALOG.md).");
            return Task.CompletedTask;
        }
    }
}
