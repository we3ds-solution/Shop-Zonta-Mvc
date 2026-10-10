using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Enums;

namespace market_mvc.Domain.ViewModels.Product
{
    public class ProductEditVM
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Compare at price must be greater than 0")]
        public decimal? CompareAtPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0")]
        public decimal? CostPrice { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        [StringLength(200, ErrorMessage = "Image alt text cannot exceed 200 characters")]
        public string? ImageAlt { get; set; }

        [StringLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
        public string? Sku { get; set; }

        [StringLength(100, ErrorMessage = "Barcode cannot exceed 100 characters")]
        public string? Barcode { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative")]
        public int? LowStockThreshold { get; set; }

        public bool TrackQuantity { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Weight cannot be negative")]
        public decimal? Weight { get; set; }

        [StringLength(10, ErrorMessage = "Weight unit cannot exceed 10 characters")]
        public string? WeightUnit { get; set; } = "kg";

        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        public bool IsFeatured { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        [StringLength(200, ErrorMessage = "Meta title cannot exceed 200 characters")]
        public string? MetaTitle { get; set; }

        [StringLength(500, ErrorMessage = "Meta description cannot exceed 500 characters")]
        public string? MetaDescription { get; set; }

        [StringLength(1000, ErrorMessage = "Tags cannot exceed 1000 characters")]
        public string? Tags { get; set; }

        public Guid? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Current image for display
        public string? CurrentImageUrl { get; set; }
    }
}

