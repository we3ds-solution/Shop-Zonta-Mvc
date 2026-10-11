using System.Net;
using Microsoft.Extensions.DependencyInjection;
using market_mvc.Infrastructure.Data;

namespace Zonta.Tests;

public class StorefrontIntegrationTests : IClassFixture<ZontaWebFactory>
{
    private readonly ZontaWebFactory _factory;

    public StorefrontIntegrationTests(ZontaWebFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Home_Returns_Brand_Storefront()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("ZONTA", html);
    }

    [Fact]
    public async Task Products_Returns_Catalog()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Checkout_Anonymous_Redirects_To_Login()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var response = await client.GetAsync("/Cart/Checkout");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Auth/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Register_Then_Profile_Works()
    {
        var (client, _) = await _factory.RegisterAsync();
        var profile = await client.GetAsync("/Auth/Profile");
        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
    }

    [Fact]
    public async Task Customer_Cannot_Create_Product()
    {
        var (client, _) = await _factory.RegisterAsync();
        var response = await client.GetAsync("/Products/Create");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(
            response.RequestMessage!.RequestUri!.ToString().Contains("AccessDenied"),
            "customer must land on AccessDenied, got " + response.RequestMessage.RequestUri);
    }

    [Fact]
    public async Task Customer_Cannot_View_Others_Order()
    {
        var productId = await _factory.SeedProductAsync(1000m);
        var (clientA, emailA) = await _factory.RegisterAsync();
        var orderId = await PlaceOrderAsync(clientA, emailA, productId, 2);

        var (clientB, _) = await _factory.RegisterAsync();
        var details = await clientB.GetAsync($"/Orders/Details/{orderId}");
        Assert.Equal(HttpStatusCode.NotFound, details.StatusCode);
    }

    [Fact]
    public async Task Owner_Can_View_Own_Order_With_Server_Total()
    {
        var productId = await _factory.SeedProductAsync(1000m);
        var (client, email) = await _factory.RegisterAsync();
        var orderId = await PlaceOrderAsync(client, email, productId, 2);

        var details = await client.GetAsync($"/Orders/Details/{orderId}");
        Assert.Equal(HttpStatusCode.OK, details.StatusCode);
        var html = await details.Content.ReadAsStringAsync();
        // 2 x 1000 + free shipping (>= 750) = 2,000.00, calculated server-side.
        Assert.Contains("2,000.00", html);
    }

    [Fact]
    public async Task ForgotPassword_Same_Response_For_Unknown_Email()
    {
        var client = _factory.CreateClient();
        var get = await client.GetAsync("/Auth/ForgotPassword");
        get.EnsureSuccessStatusCode();
        var token = ZontaWebFactory.AntiforgeryToken(await get.Content.ReadAsStringAsync());
        var post = await client.PostAsync("/Auth/ForgotPassword", new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["Email"] = "nobody-knows@example.com",
                ["__RequestVerificationToken"] = token
            }));
        Assert.Equal(HttpStatusCode.OK, post.StatusCode);
        Assert.Contains("ForgotPasswordConfirmation", post.RequestMessage!.RequestUri!.ToString());
    }

    private async Task<Guid> PlaceOrderAsync(HttpClient client, string email, Guid productId, int qty)
    {
        var cartPage = await client.GetAsync("/Cart");
        cartPage.EnsureSuccessStatusCode();
        var addToken = ZontaWebFactory.AntiforgeryToken(await cartPage.Content.ReadAsStringAsync());
        var add = await client.PostAsync("/Cart/Add", new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["ProductId"] = productId.ToString(),
                ["Quantity"] = qty.ToString(),
                ["__RequestVerificationToken"] = addToken
            }));
        add.EnsureSuccessStatusCode();

        var checkoutPage = await client.GetAsync("/Cart/Checkout");
        checkoutPage.EnsureSuccessStatusCode();
        var checkoutToken = ZontaWebFactory.AntiforgeryToken(await checkoutPage.Content.ReadAsStringAsync());
        var checkout = await client.PostAsync("/Cart/Checkout", new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["FirstName"] = "Test",
                ["LastName"] = "User",
                ["Email"] = email,
                ["Street"] = "1 Nile St",
                ["City"] = "Cairo",
                ["State"] = "Cairo",
                ["ZipCode"] = "11511",
                ["Country"] = "Egypt",
                ["__RequestVerificationToken"] = checkoutToken
            }));
        checkout.EnsureSuccessStatusCode();
        var html = await checkout.Content.ReadAsStringAsync();
        var m = System.Text.RegularExpressions.Regex.Match(html, "/Orders/Details/([0-9a-fA-F-]+)");
        Assert.True(m.Success, "order confirmation should link the order, got: " + checkout.RequestMessage!.RequestUri);
        return Guid.Parse(m.Groups[1].Value);
    }
}
