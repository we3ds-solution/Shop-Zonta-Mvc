namespace market_mvc.Features.Shipping
{
    /// <summary>
    /// Verified rule (official announcement bar, archived 2024-12-21):
    /// "شحن مجاني لجميع الطلبات فوق 750 جنيه" — free shipping over 750 EGP.
    /// Below-threshold flat fee comes from config (Zonta:ShippingFee, default 0
    /// until the business confirms it). Single source of truth for cart + checkout.
    /// </summary>
    public static class ZontaShipping
    {
        public const decimal VerifiedFreeThreshold = 750m;

        public static decimal Threshold(IConfiguration config) =>
            ThresholdValue(config.GetValue<decimal?>("Zonta:FreeShippingThreshold"));

        public static decimal FlatFee(IConfiguration config) =>
            Math.Max(0, config.GetValue<decimal>("Zonta:ShippingFee", 0m));

        public static (decimal Amount, bool IsFree) Compute(decimal subTotal, IConfiguration config) =>
            Compute(subTotal, FlatFee(config), Threshold(config));

        public static decimal ThresholdValue(decimal? configured) =>
            configured is { } t && t > 0 ? t : VerifiedFreeThreshold;

        public static (decimal Amount, bool IsFree) Compute(decimal subTotal, decimal flatFee, decimal threshold)
        {
            var fee = Math.Max(0, flatFee);
            if (subTotal >= threshold) return (0m, true);
            return (fee, false);
        }
    }
}
