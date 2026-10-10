using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Domain.entity;

namespace market_mvc.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for OrderItem entity
    /// </summary>
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> entity)
        {
            // Configure decimal properties with precision
            entity.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(oi => oi.Quantity)
                .IsRequired();

            // Configure composite indexes
            entity.HasIndex(oi => new { oi.OrderId, oi.ProductId })
                .HasDatabaseName("IX_OrderItem_OrderId_ProductId");

            // Configure regular indexes for performance
            entity.HasIndex(oi => oi.OrderId)
                .HasDatabaseName("IX_OrderItem_OrderId");

            entity.HasIndex(oi => oi.ProductId)
                .HasDatabaseName("IX_OrderItem_ProductId");

            entity.Property(oi => oi.ProductSku).HasMaxLength(100);
            entity.Property(oi => oi.VariantLabel).HasMaxLength(100);

            entity.HasIndex(oi => oi.ProductVariantId)
                .HasDatabaseName("IX_OrderItem_VariantId");

            entity.HasIndex(oi => oi.IsDeleted)
                .HasDatabaseName("IX_OrderItem_IsDeleted");

            // Configure relationship with Order
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItem_Order");

            // Configure relationship with Product
            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_OrderItem_Product");
        }
    }
}


