using market_mvc.Domain.Enums;

namespace market_mvc.Features.Payments
{
    public record PaymentResult(bool Ok, PaymentStatus Status, string? TransactionId, string? Error);

    /// <summary>
    /// Payment abstraction so future providers (Paymob/Fawry/Stripe) plug in
    /// without rewriting checkout. Cash on Delivery stays the default.
    /// No provider collects or stores raw card details in this codebase.
    /// </summary>
    public interface IPaymentProvider
    {
        string MethodName { get; }
        bool IsOnline { get; }
        Task<PaymentResult> CreateChargeAsync(decimal amount, string currency, CancellationToken ct = default);
    }

    /// <summary>
    /// Cash on Delivery: no online charge; payment stays Pending until collected.
    /// </summary>
    public class CashOnDeliveryProvider : IPaymentProvider
    {
        public string MethodName => "Cash on Delivery";
        public bool IsOnline => false;

        public Task<PaymentResult> CreateChargeAsync(decimal amount, string currency, CancellationToken ct = default)
        {
            if (amount <= 0) return Task.FromResult(new PaymentResult(false, PaymentStatus.Failed, null, "Invalid amount."));
            return Task.FromResult(new PaymentResult(true, PaymentStatus.Pending, null, null));
        }
    }
}
