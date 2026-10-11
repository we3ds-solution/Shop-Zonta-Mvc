using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Enums;
using market_mvc.Resources;

namespace market_mvc.Domain.ViewModels.Product
{
    /// <summary>Validation messages follow the current UI culture (EN/AR).</summary>
    public class ProductCreateVM
    {
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Name_Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Name_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Name_Display")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Desc_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Desc_Display")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Price_Required")]
        [Range(0.01, double.MaxValue, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Price_Range")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Price_Display")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Compare_Range")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Compare_Display")]
        public decimal? CompareAtPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Cost_Range")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Cost_Display")]
        public decimal? CostPrice { get; set; }

        [Url(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Url_Invalid")]
        [StringLength(500)]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_ImageUrl_Display")]
        public string? ImageUrl { get; set; }

        [Display(ResourceType = typeof(SharedValidation), Name = "Product_ImageFile_Display")]
        public IFormFile? ImageFile { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Alt_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Alt_Display")]
        public string? ImageAlt { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Sku_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Sku_Display")]
        public string? Sku { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Barcode_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Barcode_Display")]
        public string? Barcode { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Stock_Range")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Stock_Display")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_LowStock_Range")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_LowStock_Display")]
        public int? LowStockThreshold { get; set; }

        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Track_Display")]
        public bool TrackQuantity { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Weight_Range")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Weight_Display")]
        public decimal? Weight { get; set; }

        [StringLength(10, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_WeightUnit_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_WeightUnit_Display")]
        public string? WeightUnit { get; set; } = "kg";

        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Status_Display")]
        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Featured_Display")]
        public bool IsFeatured { get; set; } = false;

        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Sort_Display")]
        public int SortOrder { get; set; } = 0;

        [StringLength(200, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_MetaTitle_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_MetaTitle_Display")]
        public string? MetaTitle { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_MetaDesc_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_MetaDesc_Display")]
        public string? MetaDescription { get; set; }

        [StringLength(1000, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Product_Tags_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Tags_Display")]
        public string? Tags { get; set; }

        [Display(ResourceType = typeof(SharedValidation), Name = "Product_Category_Display")]
        public Guid? CategoryId { get; set; }
    }
}
