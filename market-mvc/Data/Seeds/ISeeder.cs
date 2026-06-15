using Microsoft.EntityFrameworkCore;

namespace market_mvc.Data.Seeds
{
    /// <summary>
    /// Interface for database seeders that implement specific data seeding logic
    /// </summary>
    public interface ISeeder
    {
        /// <summary>
        /// Gets the display name of this seeder
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the order priority for seeding (lower numbers execute first)
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Seeds data to the database asynchronously
        /// </summary>
        /// <param name="context">The database context</param>
        /// <returns>A task representing the async operation</returns>
        Task SeedAsync(DbContext context);
    }
}

