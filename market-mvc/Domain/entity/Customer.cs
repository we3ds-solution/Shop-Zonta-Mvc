using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Base;
using market_mvc.Domain.ObjectValues;

namespace market_mvc.Domain.entity
{
    public class Customer : BaseEntity
    {
        // Personal Information using Value Object
        public PersonName Name { get; set; } = new();
        
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        // Address using Value Object
        public Address? Address { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime? LastLoginAt { get; set; }
        
        public DateTime? EmailVerifiedAt { get; set; }
        
        public DateTime? PhoneVerifiedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // Computed properties
        public string FullName => Name.FullName;
        public bool HasAddress => Address != null;
        public bool IsEmailVerified => EmailVerifiedAt.HasValue;
        public bool IsPhoneVerified => PhoneVerifiedAt.HasValue;
        public int TotalOrders => Orders?.Count(o => !o.IsDeleted) ?? 0;
        public decimal TotalSpent => Orders?.Where(o => !o.IsDeleted).Sum(o => o.Total) ?? 0;
        
        public string StatusBadgeClass => IsActive switch
        {
            true when IsDeleted => "bg-danger",
            true => "bg-success",
            false => "bg-secondary"
        };
        
        // Business methods
        public void VerifyEmail()
        {
            EmailVerifiedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void VerifyPhone()
        {
            PhoneVerifiedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void Deactivate(string? reason = null)
        {
            IsActive = false;
            if (!string.IsNullOrEmpty(reason))
            {
                Notes = string.IsNullOrEmpty(Notes) ? $"Deactivated: {reason}" : $"{Notes}\nDeactivated: {reason}";
            }
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

