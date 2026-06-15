using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Models.entity;

namespace market_mvc.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for Order entity
    /// </summary>
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            // Configure decimal properties with precision
            entity.Property(o => o.SubTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.TaxAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.ShippingAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.DiscountAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure enums as integers
            entity.Property(o => o.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(o => o.PaymentStatus)
                .HasConversion<int>()
                .IsRequired();

            // Configure string properties max length
            entity.Property(o => o.OrderNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(o => o.CustomerEmail)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(o => o.CustomerPhone)
                .HasMaxLength(20);

            entity.Property(o => o.PaymentMethod)
                .HasMaxLength(100);

            entity.Property(o => o.PaymentTransactionId)
                .HasMaxLength(200);

            entity.Property(o => o.TrackingNumber)
                .HasMaxLength(100);

            // Configure PersonName value object
            entity.OwnsOne(o => o.CustomerName, name =>
            {
                name.Property(n => n.FirstName)
                    .HasColumnName("CustomerFirstName")
                    .HasMaxLength(100)
                    .IsRequired();

                name.Property(n => n.LastName)
                    .HasColumnName("CustomerLastName")
                    .HasMaxLength(100)
                    .IsRequired();

                name.Property(n => n.MiddleName)
                    .HasColumnName("CustomerMiddleName")
                    .HasMaxLength(100);
            });

            // Configure Shipping Address value object
            entity.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("ShippingStreet")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("ShippingCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("ShippingState")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.ZipCode)
                    .HasColumnName("ShippingZipCode")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("ShippingCountry")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // Configure Billing Address value object (optional)
            entity.OwnsOne(o => o.BillingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("BillingStreet")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("BillingCity")
                    .HasMaxLength(100);

                address.Property(a => a.State)
                    .HasColumnName("BillingState")
                    .HasMaxLength(100);

                address.Property(a => a.ZipCode)
                    .HasColumnName("BillingZipCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("BillingCountry")
                    .HasMaxLength(100);
            });

            // Configure unique indexes
            entity.HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasName("IX_Order_OrderNumber_Unique");

            // Configure regular indexes for performance
            entity.HasIndex(o => o.CustomerEmail)
                .HasName("IX_Order_CustomerEmail");

            entity.HasIndex(o => o.Status)
                .HasName("IX_Order_Status");

            entity.HasIndex(o => o.PaymentStatus)
                .HasName("IX_Order_PaymentStatus");

            entity.HasIndex(o => o.CreatedAt)
                .HasName("IX_Order_CreatedAt");

            entity.HasIndex(o => o.IsDeleted)
                .HasName("IX_Order_IsDeleted");

            entity.HasIndex(o => o.CustomerId)
                .HasName("IX_Order_CustomerId");

            // Configure relationship with Customer
            entity.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Order_Customer");
        }
    }
}
