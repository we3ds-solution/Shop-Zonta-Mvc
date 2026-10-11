using System.Net;
using System.Text;
using market_mvc.Features.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zonta.Tests;

public sealed class FakePaymobHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses = new();
    public List<string> RequestUrls { get; } = new();

    public void EnqueueJson(string json, HttpStatusCode code = HttpStatusCode.OK) =>
        _responses.Enqueue(new HttpResponseMessage(code)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        RequestUrls.Add(request.RequestUri!.ToString());
        return Task.FromResult(_responses.Dequeue());
    }
}

public class PaymobProviderTests
{
    public static IConfiguration FakeConfig(Dictionary<string, string?> values)
    {
        // Group flat "Section:Key" pairs into one JSON object per section.
        var sections = new Dictionary<string, List<KeyValuePair<string, string?>>>();
        foreach (var kv in values)
        {
            var parts = kv.Key.Split(':', 2);
            var section = parts.Length == 2 ? parts[0] : "";
            var key = parts.Length == 2 ? parts[1] : parts[0];
            if (!sections.TryGetValue(section, out var list))
            {
                list = new List<KeyValuePair<string, string?>>();
                sections[section] = list;
            }
            list.Add(new KeyValuePair<string, string?>(key, kv.Value));
        }
        static void AppendValue(System.Text.StringBuilder sb, string? v) =>
            sb.Append(v == null ? "null" : "\"" + v.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"");
        var sb = new System.Text.StringBuilder("{");
        var firstSection = true;
        foreach (var section in sections)
        {
            if (!firstSection) sb.Append(',');
            firstSection = false;
            if (section.Key == "")
            {
                var first = true;
                foreach (var kv in section.Value)
                {
                    if (!first) sb.Append(',');
                    first = false;
                    sb.Append('"').Append(kv.Key).Append("\":");
                    AppendValue(sb, kv.Value);
                }
            }
            else
            {
                sb.Append('"').Append(section.Key).Append("\":{");
                var first = true;
                foreach (var kv in section.Value)
                {
                    if (!first) sb.Append(',');
                    first = false;
                    sb.Append('"').Append(kv.Key).Append("\":");
                    AppendValue(sb, kv.Value);
                }
                sb.Append('}');
            }
        }
        sb.Append('}');
        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        return new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(bytes))
            .Build();
    }

    private static (PaymobProvider Provider, FakePaymobHandler Handler) Create()
    {
        IConfiguration config = FakeConfig(new Dictionary<string, string?>
        {
            ["Paymob:ApiKey"] = "test-key",
            ["Paymob:IntegrationId"] = "123",
            ["Paymob:IframeId"] = "456"
        });
        var handler = new FakePaymobHandler();
        var http = new HttpClient(handler);
        return (new PaymobProvider(http, config, NullLogger<PaymobProvider>.Instance), handler);
    }

    [Fact]
    public async Task CreateCharge_FullFlow_Returns_Pending_With_Iframe_Url()
    {
        var (provider, handler) = Create();
        Assert.True(provider.IsOnline);
        Assert.True(provider.IsConfigured);
        handler.EnqueueJson("""{"token":"auth-tok"}""");
        handler.EnqueueJson("""{"id":789}""");
        handler.EnqueueJson("""{"token":"pay-key"}""");

        var result = await provider.CreateChargeAsync(1900m, "EGP");

        Assert.True(result.Ok);
        Assert.Equal(market_mvc.Domain.Enums.PaymentStatus.Pending, result.Status);
        Assert.Equal("789", result.TransactionId);
        Assert.Equal("https://accept.paymob.com/api/acceptance/iframes/456?payment_token=pay-key", result.RedirectUrl);
        Assert.Equal(3, handler.RequestUrls.Count);
    }

    [Fact]
    public async Task CreateCharge_Unconfigured_Fails_Closed()
    {
        IConfiguration config = PaymobProviderTests.FakeConfig(new Dictionary<string, string?>());
        var provider = new PaymobProvider(
            new HttpClient(new FakePaymobHandler()), config, NullLogger<PaymobProvider>.Instance);
        Assert.False(provider.IsConfigured);
        var result = await provider.CreateChargeAsync(100m, "EGP");
        Assert.False(result.Ok);
    }

    [Fact]
    public async Task CreateCharge_Gateway_Error_Maps_To_Failed()
    {
        var (provider, handler) = Create();
        handler.EnqueueJson("""{"error":"nope"}""", HttpStatusCode.InternalServerError);
        var result = await provider.CreateChargeAsync(100m, "EGP");
        Assert.False(result.Ok);
        Assert.Equal(market_mvc.Domain.Enums.PaymentStatus.Failed, result.Status);
    }
}

