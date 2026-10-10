using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Base;

namespace market_mvc.Domain.entity
{
    /// <summary>
    /// Smallest maintainable apparel-variant support: optional size/color per product.
    /// Products without variants keep using Product.Price/StockQuantity.
    /// Variant stock is authoritative when variants exist and are active.
    /// </summary>
    public class ProductVariant : BaseEntity
    {
        [Required]
        public Guid ProductId { get; set; }

        [StringLength(20)]
        public string? Size { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(100)]
        public string? Sku { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        /// <summary>Optional per-variant price; null means use Product.Price.</summary>
        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual Product Product { get; set; } = null!;

        public string Label
        {
            get
            {
                var parts = new[] { Size, Color }.Where(s => !string.IsNullOrWhiteSpace(s));
                var label = string.Join(" / ", parts);
                return string.IsNullOrWhiteSpace(label) ? "Standard" : label;
            }
        }

        public decimal EffectivePrice(decimal productPrice) => Price ?? productPrice;

        public bool CanFulfill(int qty) => StockQuantity >= qty;
    }
}
