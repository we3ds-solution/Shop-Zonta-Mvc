using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.entity
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public string? UpdatedBy { get; set; }
        
        // Soft Delete
        public bool IsDeleted { get; set; } = false;
        
        public DateTime? DeletedAt { get; set; }
        
        public string? DeletedBy { get; set; }
        
        // Concurrency control
        [Timestamp]
        public byte[]? RowVersion { get; set; }
        
        // Helper methods
        public void MarkAsDeleted(string? deletedBy = null)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = deletedBy;
        }
        
        public void Restore(string? restoredBy = null)
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = restoredBy;
        }
        
        public void UpdateTimestamp(string? updatedBy = null)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}

