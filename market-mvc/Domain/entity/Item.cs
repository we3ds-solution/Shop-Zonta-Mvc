using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.entity;

namespace market_mvc.Domain.entity
{
    public class Item : BaseEntity
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Sku { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;

        // Computed properties
        public bool IsInStock => Quantity > 0;
        public string StatusBadgeClass => IsActive switch
        {
            true when IsDeleted => "bg-danger",
            true when IsInStock => "bg-success",
            true => "bg-warning",
            false => "bg-secondary"
        };
    }
}

