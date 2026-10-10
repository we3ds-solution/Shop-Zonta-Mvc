using market_mvc.Infrastructure.Common;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;

namespace market_mvc.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : BaseCommand<Product>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public int StockQuantity { get; set; }
        public int? LowStockThreshold { get; set; }
        public bool TrackQuantity { get; set; } = true;
        public decimal? Weight { get; set; }
        public string? WeightUnit { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Draft;
        public bool IsFeatured { get; set; }
        public int SortOrder { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Tags { get; set; }
        public Guid? CategoryId { get; set; }
    }
}

