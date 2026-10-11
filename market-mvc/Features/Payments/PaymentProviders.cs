using market_mvc.Domain.Enums;

namespace market_mvc.Features.Payments
{
    public record PaymentResult(bool Ok, PaymentStatus Status, string? TransactionId, string? Error, string? RedirectUrl = null);

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

    /// <summary>
    /// Paymob Accept (Egypt) card/wallet flow: auth token, order registration,
    /// then payment key; the shopper completes payment in Paymob's hosted iframe.
    /// Secrets come ONLY from configuration (Paymob:ApiKey/IntegrationId/IframeId/HmacSecret);
    /// without them the provider refuses (fail-closed). No raw card data touches us.
    /// Docs: https://docs.paymob.com/docs/accept-standard-redirect
    /// </summary>
    public class PaymobProvider : IPaymentProvider
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymobProvider> _logger;

        public PaymobProvider(HttpClient http, IConfiguration config, ILogger<PaymobProvider> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }

        public string MethodName => "Paymob";
        public bool IsOnline => true;

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(_config["Paymob:ApiKey"]) &&
            !string.IsNullOrWhiteSpace(_config["Paymob:IntegrationId"]);

        public async Task<PaymentResult> CreateChargeAsync(decimal amount, string currency, CancellationToken ct = default)
        {
            if (amount <= 0) return new PaymentResult(false, PaymentStatus.Failed, null, "Invalid amount.");
            var apiKey = _config["Paymob:ApiKey"];
            var integrationId = _config["Paymob:IntegrationId"];
            var iframeId = _config["Paymob:IframeId"];
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(integrationId) || string.IsNullOrWhiteSpace(iframeId))
                return new PaymentResult(false, PaymentStatus.Failed, null, "Paymob is not configured.");

            try
            {
                var amountCents = ((long)Math.Round(amount * 100)).ToString();
                var auth = await PostJsonAsync("https://accept.paymob.com/api/auth/tokens",
                    new { api_key = apiKey }, ct);
                var authToken = auth.GetProperty("token").GetString();

                var order = await PostJsonAsync("https://accept.paymob.com/api/ecommerce/orders",
                    new
                    {
                        auth_token = authToken,
                        delivery_needed = "false",
                        amount_cents = amountCents,
                        currency = currency,
                        items = Array.Empty<object>()
                    }, ct);
                var orderId = order.GetProperty("id").GetInt32();

                var key = await PostJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys",
                    new
                    {
                        auth_token = authToken,
                        amount_cents = amountCents,
                        expiration = 3600,
                        order_id = orderId,
                        billing_data = new
                        {
                            first_name = "Zonta",
                            last_name = "Customer",
                            email = "customer@example.com",
                            phone_number = "+200000000000",
                            country = "EG",
                            city = "Cairo",
                            state = "Cairo",
                            street = "NA",
                            building = "NA",
                            floor = "NA",
                            apartment = "NA"
                        },
                        currency = currency,
                        integration_id = integrationId
                    }, ct);
                var paymentToken = key.GetProperty("token").GetString();
                var redirect = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";
                _logger.LogInformation("Paymob payment key created for order {OrderId}.", orderId);
                return new PaymentResult(true, PaymentStatus.Pending, orderId.ToString(), null, redirect);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paymob charge failed.");
                return new PaymentResult(false, PaymentStatus.Failed, null, "Payment provider error.");
            }
        }

        private async Task<System.Text.Json.JsonElement> PostJsonAsync(string url, object payload, CancellationToken ct)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            using var response = await _http.PostAsync(url, content, ct);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync(ct);
            return System.Text.Json.JsonDocument.Parse(body).RootElement;
        }
    }
}

