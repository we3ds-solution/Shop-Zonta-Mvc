using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Base;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ObjectValues;

namespace market_mvc.Domain.entity
{
    public class Order : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        public Guid? CustomerId { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        
        // Customer Name using Value Object
        public PersonName CustomerName { get; set; } = new();
        
        // Shipping Address using Value Object
        public Address ShippingAddress { get; set; } = new();
        
        // Billing Address using Value Object (optional)
        public Address? BillingAddress { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal SubTotal { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal ShippingAmount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        
        [StringLength(100)]
        public string? PaymentMethod { get; set; }
        
        [StringLength(200)]
        public string? PaymentTransactionId { get; set; }
        
        [StringLength(100)]
        public string? TrackingNumber { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed properties
        public int TotalItems => OrderItems?.Sum(oi => oi.Quantity) ?? 0;
        public int UniqueItems => OrderItems?.Count ?? 0;
        public bool HasBillingAddress => BillingAddress != null;
        public string StatusBadgeClass => Status switch
        {
            OrderStatus.Pending => "bg-warning",
            OrderStatus.Confirmed => "bg-info",
            OrderStatus.Processing => "bg-primary",
            OrderStatus.Shipped => "bg-secondary",
            OrderStatus.Delivered => "bg-success",
            OrderStatus.Cancelled => "bg-danger",
            OrderStatus.Refunded => "bg-dark",
            _ => "bg-light"
        };
        
        public string PaymentStatusBadgeClass => PaymentStatus switch
        {
            PaymentStatus.Pending => "bg-warning",
            PaymentStatus.Paid => "bg-success",
            PaymentStatus.Failed => "bg-danger",
            PaymentStatus.Refunded => "bg-info",
            PaymentStatus.PartiallyRefunded => "bg-secondary",
            _ => "bg-light"
        };
        
        // Business methods
        public void CalculateTotal()
        {
            Total = SubTotal + TaxAmount + ShippingAmount - DiscountAmount;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void MarkAsShipped(string? trackingNumber = null)
        {
            Status = OrderStatus.Shipped;
            ShippedAt = DateTime.UtcNow;
            TrackingNumber = trackingNumber;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void MarkAsDelivered()
        {
            Status = OrderStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void Cancel(string? reason = null)
        {
            Status = OrderStatus.Cancelled;
            if (!string.IsNullOrEmpty(reason))
            {
                Notes = string.IsNullOrEmpty(Notes) ? $"Cancelled: {reason}" : $"{Notes}\nCancelled: {reason}";
            }
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

