using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ViewModels.Order
{
    /// <summary>
    /// Legacy insecure VM (trusted client total). Retained for compilation only;
    /// order creation now goes through Cart checkout with server-calculated totals.
    /// </summary>
    [Obsolete("Use Cart checkout. Client-supplied totals are not accepted.")]
    public class CreateOrderVM
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Total must be greater than zero")]
        public decimal Total { get; set; }

        public Guid? OrderId { get; set; }
    }
}

