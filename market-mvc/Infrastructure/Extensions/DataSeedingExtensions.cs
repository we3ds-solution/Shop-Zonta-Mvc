using market_mvc.Infrastructure.Data;

namespace market_mvc.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for database seeding
    /// Initializes the database with seed data
    /// </summary>
    public static class DataSeedingExtensions
    {
        /// <summary>
        /// Execute database seeding on application startup
        /// </summary>
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var orchestrator = scope.ServiceProvider.GetRequiredService<SeederOrchestrator>();

                // Register all seeders
                var seeders = scope.ServiceProvider.GetServices<ISeeder>();
                foreach (var seeder in seeders)
                {
                    orchestrator.RegisterSeeder(seeder);
                }

                // Execute seeding
                try
                {
                    logger.LogInformation("Starting database seeding process");
                    await orchestrator.ExecuteAsync(context);
                    logger.LogInformation("Database seeding completed successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during database seeding");
                }
            }
        }
    }
}
