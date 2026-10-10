using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// Orchestrates idempotent seeders in priority order.
    /// Does NOT create schema: migrations are applied explicitly by DataSeedingExtensions.
    /// </summary>
    public class SeederOrchestrator
    {
        private readonly List<ISeeder> _seeders;
        private readonly ILogger<SeederOrchestrator> _logger;

        public SeederOrchestrator(ILogger<SeederOrchestrator> logger)
        {
            _logger = logger;
            _seeders = new List<ISeeder>();
        }

        public void RegisterSeeder(ISeeder seeder)
        {
            if (seeder == null)
                throw new ArgumentNullException(nameof(seeder));

            if (_seeders.Any(s => s.GetType() == seeder.GetType()))
            {
                _logger.LogWarning("Seeder {Name} is already registered", seeder.Name);
                return;
            }

            _seeders.Add(seeder);
            _logger.LogInformation("Registered seeder: {Name}", seeder.Name);
        }

        public async Task ExecuteAsync(DbContext context, CancellationToken cancellationToken = default)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sortedSeeders = _seeders.OrderBy(s => s.Order).ToList();
            if (!sortedSeeders.Any())
            {
                _logger.LogWarning("No seeders registered");
                return;
            }

            _logger.LogInformation("Starting seeding process with {Count} seeders", sortedSeeders.Count);

            foreach (var seeder in sortedSeeders)
            {
                try
                {
                    _logger.LogInformation("[{Order}] Executing seeder: {Name}", seeder.Order, seeder.Name);
                    await seeder.SeedAsync(context);
                    _logger.LogInformation("[{Order}] Completed seeder: {Name}", seeder.Order, seeder.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing seeder: {Name}", seeder.Name);
                    throw;
                }
            }

            _logger.LogInformation("Seeding process completed successfully");
        }

        public IEnumerable<ISeeder> GetRegisteredSeeders() => _seeders.OrderBy(s => s.Order);

        public int SeederCount => _seeders.Count;
    }
}
