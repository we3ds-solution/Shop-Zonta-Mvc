using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Models.entity;

namespace market_mvc.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for Item entity
    /// </summary>
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> entity)
        {
            // Configure string properties max length
            entity.Property(i => i.Name)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(i => i.Description)
                .HasMaxLength(2000);

            entity.Property(i => i.Sku)
                .HasMaxLength(100);

            // Configure decimal properties with precision
            entity.Property(i => i.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure unique indexes
            entity.HasIndex(i => i.Sku)
                .IsUnique()
                .HasFilter("[Sku] IS NOT NULL AND [IsDeleted] = 0")
                .HasName("IX_Item_Sku_Unique");

            // Configure regular indexes for performance
            entity.HasIndex(i => i.IsActive)
                .HasName("IX_Item_IsActive");

            entity.HasIndex(i => i.IsDeleted)
                .HasName("IX_Item_IsDeleted");

            entity.HasIndex(i => i.CreatedAt)
                .HasName("IX_Item_CreatedAt");
        }
    }
}
