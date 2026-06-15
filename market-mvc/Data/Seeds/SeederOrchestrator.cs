using Microsoft.EntityFrameworkCore;
using market_mvc.Data;

namespace market_mvc.Data.Seeds
{
    /// <summary>
    /// Orchestrates the execution of all seeders in the correct order
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

        /// <summary>
        /// Registers a seeder for execution
        /// </summary>
        public void RegisterSeeder(ISeeder seeder)
        {
            if (seeder == null)
                throw new ArgumentNullException(nameof(seeder));

            if (_seeders.Any(s => s.GetType() == seeder.GetType()))
            {
                _logger.LogWarning($"Seeder {seeder.Name} is already registered");
                return;
            }

            _seeders.Add(seeder);
            _logger.LogInformation($"Registered seeder: {seeder.Name}");
        }

        /// <summary>
        /// Seeds all registered seeders in priority order
        /// </summary>
        public async Task ExecuteAsync(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database ensured created");

                // Sort seeders by order priority
                var sortedSeeders = _seeders.OrderBy(s => s.Order).ToList();

                if (!sortedSeeders.Any())
                {
                    _logger.LogWarning("No seeders registered");
                    return;
                }

                _logger.LogInformation($"Starting seeding process with {sortedSeeders.Count} seeders");

                foreach (var seeder in sortedSeeders)
                {
                    try
                    {
                        _logger.LogInformation($"[{seeder.Order}] Executing seeder: {seeder.Name}");
                        await seeder.SeedAsync(context);
                        _logger.LogInformation($"[{seeder.Order}] Completed seeder: {seeder.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error executing seeder: {seeder.Name}");
                        throw;
                    }
                }

                _logger.LogInformation("Seeding process completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during seeding process");
                throw;
            }
        }

        /// <summary>
        /// Gets the list of registered seeders in execution order
        /// </summary>
        public IEnumerable<ISeeder> GetRegisteredSeeders() => _seeders.OrderBy(s => s.Order);

        /// <summary>
        /// Gets the count of registered seeders
        /// </summary>
        public int SeederCount => _seeders.Count;
    }
}

