using Microsoft.AspNetCore.Identity;

namespace market_mvc.Domain.entity
{
    /// <summary>
    /// Application user entity extending ASP.NET Core Identity
    /// Handles authentication, passwords, and two-factor authentication
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Foreign key to Customer entity for business profile
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Navigation property to Customer business entity
        /// </summary>
        public virtual Customer? Customer { get; set; }

        /// <summary>
        /// User's full name (for display purposes)
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Account creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last password change timestamp
        /// </summary>
        public DateTime? LastPasswordChangedAt { get; set; }

        /// <summary>
        /// Last login timestamp (supplementing Identity's LastLogin)
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Account lockout reason (if manually locked)
        /// </summary>
        public string? LockoutReason { get; set; }

        /// <summary>
        /// User roles as comma-separated string (denormalized for quick access)
        /// </summary>
        public string? Roles { get; set; }
    }
}
