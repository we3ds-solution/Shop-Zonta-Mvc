using Microsoft.EntityFrameworkCore;

namespace market_mvc.Data
{
    /// <summary>
    /// Legacy DataSeeder - DEPRECATED
    /// Use SeederOrchestrator in Data.Seeds instead
    /// This class is kept for backward compatibility only
    /// </summary>
    [Obsolete("Use SeederOrchestrator from market_mvc.Data.Seeds instead", false)]
    public static class DataSeeder
    {
        /// <summary>
        /// Legacy seeding method - No longer in use
        /// All seeding is now handled by SeederOrchestrator
        /// </summary>
        public static async Task SeedAsync(AppDbContext context)
        {
            // Deprecated: All seeding is now handled by SeederOrchestrator
            // See Data/Seeds/README.md for details on the new seeding system
        }
    }
}
