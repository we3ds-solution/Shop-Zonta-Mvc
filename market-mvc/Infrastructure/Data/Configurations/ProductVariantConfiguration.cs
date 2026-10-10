using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Domain.entity;

namespace market_mvc.Infrastructure.Data.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> entity)
        {
            entity.Property(v => v.Size).HasMaxLength(20);
            entity.Property(v => v.Color).HasMaxLength(50);
            entity.Property(v => v.Sku).HasMaxLength(100);
            entity.Property(v => v.Price).HasPrecision(18, 2);
            entity.Ignore(v => v.Label);

            entity.HasIndex(v => v.ProductId).HasDatabaseName("IX_Variant_ProductId");
            entity.HasIndex(v => v.Sku).HasDatabaseName("IX_Variant_Sku");
            entity.HasIndex(v => v.IsDeleted).HasDatabaseName("IX_Variant_IsDeleted");

            entity.HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Variant_Product");
        }
    }
}
