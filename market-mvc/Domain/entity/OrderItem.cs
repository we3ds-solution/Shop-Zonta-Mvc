using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Base;

namespace market_mvc.Domain.entity
{
    public class OrderItem : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [StringLength(200)]
        public string? ProductName { get; set; } // Snapshot of product name at time of order
        
        [StringLength(100)]
        public string? ProductSku { get; set; } // Snapshot of product SKU at time of order

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;

        // Computed properties
        public decimal TotalPrice => Quantity * UnitPrice;
        public decimal LineTotal => TotalPrice; // Alias for clarity
    }
}

