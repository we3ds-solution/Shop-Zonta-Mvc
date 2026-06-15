using market_mvc.Domain.Enums;

namespace market_mvc.Domain.ViewModels.Product
{
    public class ProductCardVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public int StockQuantity { get; set; }
        public bool TrackQuantity { get; set; }
        public string? CategoryName { get; set; }

        // Computed properties
        public bool IsOnSale => CompareAtPrice.HasValue && CompareAtPrice > Price;
        public decimal? SavingsAmount => CompareAtPrice.HasValue ? CompareAtPrice - Price : null;
        public decimal? SavingsPercentage => CompareAtPrice.HasValue && CompareAtPrice > 0 
            ? Math.Round(((CompareAtPrice.Value - Price) / CompareAtPrice.Value) * 100, 2) 
            : null;
        public decimal DiscountPercentage => SavingsPercentage ?? 0;
        public bool IsOutOfStock => TrackQuantity && StockQuantity <= 0;
        public string StatusBadgeClass => Status switch
        {
            ProductStatus.Active => "bg-success",
            ProductStatus.Inactive => "bg-secondary",
            ProductStatus.OutOfStock => "bg-danger",
            ProductStatus.Discontinued => "bg-dark",
            _ => "bg-warning"
        };
    }
}

