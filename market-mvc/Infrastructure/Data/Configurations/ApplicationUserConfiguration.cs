using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Domain.entity;

namespace market_mvc.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for ApplicationUser
    /// Defines relationships, constraints, and default values
    /// </summary>
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Primary key is inherited from IdentityUser (Id)

            // Properties
            builder.Property(u => u.FullName)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.LockoutReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(u => u.Roles)
                .HasMaxLength(500)
                .IsRequired(false);

            // Relationships
            builder.HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.NormalizedEmail);
            builder.HasIndex(u => u.NormalizedUserName);
            builder.HasIndex(u => u.CustomerId);
        }
    }
}
