using System.ComponentModel.DataAnnotations;
using market_mvc.Resources;

namespace market_mvc.Domain.ViewModels.Cart
{
    public class CartLineVM
    {
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }
        [Range(1, 99, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Quantity_Range")]
        public int Quantity { get; set; } = 1;
    }

    public class CartItemVM
    {
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? VariantLabel { get; set; }
        public string? Sku { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
        public string DisplayName => VariantLabel == null ? ProductName : $"{ProductName} ({VariantLabel})";
    }

    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();
        public decimal SubTotal => Items.Sum(i => i.LineTotal);
        public decimal ShippingAmount { get; set; }
        public decimal Total => SubTotal + ShippingAmount;
        public int TotalItems => Items.Sum(i => i.Quantity);
        public bool IsEmpty => !Items.Any();
    }

    public class AddToCartVM
    {
        [Required]
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }
        [Range(1, 99, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Quantity_Range")]
        public int Quantity { get; set; } = 1;
    }

    public class CheckoutVM
    {
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "FirstName_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "FirstName_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "FirstName_Display")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "LastName_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "LastName_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "LastName_Display")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Invalid")]
        [StringLength(200, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Email_Display")]
        public string Email { get; set; } = string.Empty;
        [Phone(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Phone_Invalid")]
        [StringLength(20, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Phone_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Phone_Display")]
        public string? Phone { get; set; }
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Street_Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Street_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Street_Display")]
        public string Street { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "City_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "City_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "City_Display")]
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "State_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "State_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "State_Display")]
        public string State { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "ZipCode_Required")]
        [StringLength(20, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "ZipCode_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "ZipCode_Display")]
        public string ZipCode { get; set; } = string.Empty;
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Country_Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Country_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Country_Display")]
        public string Country { get; set; } = "Egypt";
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Display only (never trusted for totals).
        public CartVM Cart { get; set; } = new();
    }
}
