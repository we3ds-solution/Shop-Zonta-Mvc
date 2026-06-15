using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Domain.entity;

namespace market_mvc.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for Product entity
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            // Configure decimal properties with precision
            entity.Property(p => p.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(p => p.CompareAtPrice)
                .HasPrecision(18, 2);

            entity.Property(p => p.CostPrice)
                .HasPrecision(18, 2);

            entity.Property(p => p.Weight)
                .HasPrecision(10, 3);

            // Configure enums as integers
            entity.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            // Configure unique indexes
            entity.HasIndex(p => p.Sku)
                .IsUnique()
                .HasFilter("[Sku] IS NOT NULL AND [IsDeleted] = 0")
                .HasName("IX_Product_Sku_Unique");

            entity.HasIndex(p => p.Barcode)
                .IsUnique()
                .HasFilter("[Barcode] IS NOT NULL AND [IsDeleted] = 0")
                .HasName("IX_Product_Barcode_Unique");

            // Configure regular indexes for performance
            entity.HasIndex(p => p.Status)
                .HasName("IX_Product_Status");

            entity.HasIndex(p => p.IsFeatured)
                .HasName("IX_Product_IsFeatured");

            entity.HasIndex(p => p.CreatedAt)
                .HasName("IX_Product_CreatedAt");

            entity.HasIndex(p => p.IsDeleted)
                .HasName("IX_Product_IsDeleted");

            entity.HasIndex(p => p.CategoryId)
                .HasName("IX_Product_CategoryId");

            // Configure relationship with Category
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Product_Category");

            // Configure string properties max length
            entity.Property(p => p.Name)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(p => p.Description)
                .HasMaxLength(2000);

            entity.Property(p => p.Sku)
                .HasMaxLength(100);

            entity.Property(p => p.Barcode)
                .HasMaxLength(100);

            entity.Property(p => p.MetaTitle)
                .HasMaxLength(255);

            entity.Property(p => p.MetaDescription)
                .HasMaxLength(500);

            entity.Property(p => p.Tags)
                .HasMaxLength(1000);

            entity.Property(p => p.WeightUnit)
                .HasMaxLength(20);
        }
    }
}

