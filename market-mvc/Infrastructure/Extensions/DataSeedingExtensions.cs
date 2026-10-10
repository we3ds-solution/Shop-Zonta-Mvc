using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Infrastructure.Data.Seeds;

namespace market_mvc.Infrastructure.Extensions
{
    /// <summary>
    /// Explicit, environment-appropriate database initialization.
    /// Development: apply pending migrations automatically, then seed.
    /// Production: NEVER auto-migrate unless explicitly authorized via
    /// ZONTA_AUTO_MIGRATE=true (backup + reviewed plan required, see README).
    /// Fails fast on error instead of reporting successful startup.
    /// </summary>
    public static class DataSeedingExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var orchestrator = scope.ServiceProvider.GetRequiredService<SeederOrchestrator>();

            var seeders = scope.ServiceProvider.GetServices<ISeeder>();
            foreach (var seeder in seeders)
            {
                orchestrator.RegisterSeeder(seeder);
            }

            try
            {
                if (app.Environment.IsProduction())
                {
                    var autoMigrate = string.Equals(
                        Environment.GetEnvironmentVariable("ZONTA_AUTO_MIGRATE"),
                        "true", StringComparison.OrdinalIgnoreCase);
                    var pending = await context.Database.GetPendingMigrationsAsync();
                    if (pending.Any() && !autoMigrate)
                    {
                        logger.LogCritical(
                            "Pending migrations detected in production ({Migrations}) but ZONTA_AUTO_MIGRATE is not 'true'. " +
                            "Refusing to auto-migrate. Apply with backup + reviewed plan: dotnet ef database update.",
                            string.Join(", ", pending));
                        throw new InvalidOperationException(
                            "Pending production migrations require explicit authorization (ZONTA_AUTO_MIGRATE=true) with backup + review.");
                    }
                }

                logger.LogInformation("Applying pending database migrations");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied");

                logger.LogInformation("Starting database seeding process");
                await orchestrator.ExecuteAsync(context);
                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                // Do not swallow: startup must not report success when DB init failed.
                logger.LogCritical(ex, "Database initialization failed. Application will not start with an inconsistent database.");
                throw;
            }
        }
    }
}
