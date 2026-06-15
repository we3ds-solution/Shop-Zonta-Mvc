using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Models.entity;

namespace market_mvc.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for Category entity
    /// </summary>
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            // Configure string properties max length
            entity.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(c => c.Description)
                .HasMaxLength(1000);

            entity.Property(c => c.ImageUrl)
                .HasMaxLength(500);

            entity.Property(c => c.ImageAlt)
                .HasMaxLength(200);

            // Configure enums as integers
            entity.Property(c => c.Type)
                .HasConversion<int>()
                .IsRequired();

            // Configure unique indexes
            entity.HasIndex(c => c.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasName("IX_Category_Name_Unique");

            // Configure regular indexes for performance
            entity.HasIndex(c => c.Type)
                .HasName("IX_Category_Type");

            entity.HasIndex(c => c.IsActive)
                .HasName("IX_Category_IsActive");

            entity.HasIndex(c => c.IsDeleted)
                .HasName("IX_Category_IsDeleted");
        }
    }
}
