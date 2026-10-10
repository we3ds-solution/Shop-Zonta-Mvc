using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ViewModels.Cart
{
    public class CartLineVM
    {
        public Guid ProductId { get; set; }
        public Guid? VariantId { get; set; }
        [Range(1, 99)]
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
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
        public int Quantity { get; set; } = 1;
    }

    public class CheckoutVM
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;
        [Phone, StringLength(20)]
        public string? Phone { get; set; }
        [Required, StringLength(200)]
        public string Street { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;
        [Required, StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string Country { get; set; } = "Egypt";
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Display only (never trusted for totals).
        public CartVM Cart { get; set; } = new();
    }
}
