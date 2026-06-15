using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ViewModels.Order
{
    public class CreateOrderVM
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Total must be greater than zero")]
        public decimal Total { get; set; }

        public Guid? OrderId { get; set; }
    }
}

