using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using market_mvc.Models.entity;

namespace market_mvc.Data.Configurations
{
    /// <summary>
    /// Global configuration for all entities (soft delete filters, etc.)
    /// </summary>
    public static class GlobalEntityConfiguration
    {
        /// <summary>
        /// Applies global entity configurations to the model builder
        /// </summary>
        public static void ApplyGlobalConfigurations(this ModelBuilder modelBuilder)
        {
            ApplySoftDeleteFilter(modelBuilder);
        }

        /// <summary>
        /// Applies soft delete query filters to all BaseEntity descendants
        /// </summary>
        private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Check if entity inherits from BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Create expression: e => e.IsDeleted == false
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var filter = Expression.Lambda(
                        Expression.Equal(property, Expression.Constant(false)),
                        parameter
                    );

                    // Apply the filter
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}
