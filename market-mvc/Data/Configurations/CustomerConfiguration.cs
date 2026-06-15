using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Domain.entity;

namespace market_mvc.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for Customer entity
    /// </summary>
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            // Configure string properties max length
            entity.Property(c => c.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(c => c.Phone)
                .HasMaxLength(20);

            // Configure PersonName value object
            entity.OwnsOne(c => c.Name, name =>
            {
                name.Property(n => n.FirstName)
                    .HasColumnName("FirstName")
                    .HasMaxLength(100)
                    .IsRequired();

                name.Property(n => n.LastName)
                    .HasColumnName("LastName")
                    .HasMaxLength(100)
                    .IsRequired();

                name.Property(n => n.MiddleName)
                    .HasColumnName("MiddleName")
                    .HasMaxLength(100);
            });

            // Configure Address value object (optional)
            entity.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("AddressStreet")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("AddressCity")
                    .HasMaxLength(100);

                address.Property(a => a.State)
                    .HasColumnName("AddressState")
                    .HasMaxLength(100);

                address.Property(a => a.ZipCode)
                    .HasColumnName("AddressZipCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("AddressCountry")
                    .HasMaxLength(100);
            });

            // Configure unique indexes
            entity.HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasName("IX_Customer_Email_Unique");

            // Configure regular indexes for performance
            entity.HasIndex(c => c.IsActive)
                .HasName("IX_Customer_IsActive");

            entity.HasIndex(c => c.CreatedAt)
                .HasName("IX_Customer_CreatedAt");

            entity.HasIndex(c => c.IsDeleted)
                .HasName("IX_Customer_IsDeleted");
        }
    }
}

