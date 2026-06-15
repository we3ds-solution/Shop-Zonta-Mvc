using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Base;
using market_mvc.Domain.Enums;

namespace market_mvc.Domain.entity
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? CompareAtPrice { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? CostPrice { get; set; }
        
        [Url]
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [StringLength(200)]
        public string? ImageAlt { get; set; }
        
        [StringLength(100)]
        public string? Sku { get; set; }
        
        [StringLength(100)]
        public string? Barcode { get; set; }
        
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? LowStockThreshold { get; set; }
        
        public bool TrackQuantity { get; set; } = true;
        
        [Range(0, double.MaxValue)]
        public decimal? Weight { get; set; }
        
        [StringLength(10)]
        public string? WeightUnit { get; set; } = "kg";
        
        public ProductStatus Status { get; set; } = ProductStatus.Draft;
        
        public bool IsFeatured { get; set; } = false;
        
        public int SortOrder { get; set; } = 0;
        
        [StringLength(200)]
        public string? MetaTitle { get; set; }
        
        [StringLength(500)]
        public string? MetaDescription { get; set; }
        
        [StringLength(1000)]
        public string? Tags { get; set; }
        
        public Guid? CategoryId { get; set; }

        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed properties
        public bool IsOnSale => CompareAtPrice.HasValue && CompareAtPrice > Price;
        public decimal? SavingsAmount => CompareAtPrice.HasValue ? CompareAtPrice - Price : null;
        public decimal? SavingsPercentage => CompareAtPrice.HasValue && CompareAtPrice > 0 
            ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 2) 
            : null;
        public bool IsLowStock => TrackQuantity && LowStockThreshold.HasValue && StockQuantity <= LowStockThreshold;
        public bool IsOutOfStock => TrackQuantity && StockQuantity <= 0;
        
        // Business methods
        public void UpdateStock(int quantity)
        {
            if (TrackQuantity)
            {
                StockQuantity = Math.Max(0, StockQuantity + quantity);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        
        public bool CanFulfillOrder(int requestedQuantity)
        {
            return !TrackQuantity || StockQuantity >= requestedQuantity;
        }
        
        public void MarkAsFeatured()
        {
            IsFeatured = true;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void RemoveFromFeatured()
        {
            IsFeatured = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

