using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using market_mvc.Controllers;
using market_mvc.Domain.ViewModels.Cart;
using market_mvc.Features.Auth.Email;
using market_mvc.Features.Cart.Services;
using market_mvc.Features.Orders.Services;
using market_mvc.Features.Payments;
using market_mvc.Infrastructure.Localization;
using market_mvc.Infrastructure.Security;

namespace Zonta.Tests;

public class FakeCartService : ICartService
{
    public CartVM Cart { get; set; } = new();
    public List<(Guid ProductId, Guid? VariantId, int Qty)> Added { get; } = new();
    public List<(Guid ProductId, Guid? VariantId)> Removed { get; } = new();
    public bool AddResult { get; set; } = true;
    public string? AddError { get; set; }

    public List<CartLineVM> GetLines() => new();
    public Task<CartVM> GetCartAsync(CancellationToken ct = default) => Task.FromResult(Cart);
    public Task<(bool Ok, string? Error)> AddAsync(Guid productId, Guid? variantId, int quantity, CancellationToken ct = default)
    {
        if (AddResult) Added.Add((productId, variantId, quantity));
        return Task.FromResult<(bool, string?)>((AddResult, AddResult ? null : AddError ?? "Failed."));
    }
    public Task<(bool Ok, string? Error)> UpdateQuantityAsync(Guid productId, Guid? variantId, int quantity, CancellationToken ct = default)
        => Task.FromResult<(bool, string?)>((true, null));
    public void Remove(Guid productId, Guid? variantId) => Removed.Add((productId, variantId));
    public void Clear() { }
    public int Count() => 0;
}

public class CartControllerTests
{
    private static CartController Controller(FakeCartService cart)
    {
        var c = new CartController(cart, new FakeOrderService(), null!, new ZontaLocalizer(new HttpContextAccessor()))
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = ClaimsHelper.User("a@example.com") } },
            TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider())
        };
        return c;
    }

    [Fact]
    public async Task Add_Invalid_ModelState_Redirects_With_Error()
    {
        var c = Controller(new FakeCartService());
        c.ModelState.AddModelError("Quantity", "bad");
        var result = await c.Add(new AddToCartVM { ProductId = Guid.NewGuid(), Quantity = 0 }, default);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Add_Unavailable_Product_Does_Not_Add()
    {
        var cart = new FakeCartService { AddResult = false, AddError = "Out of stock." };
        var result = await Controller(cart).Add(new AddToCartVM { ProductId = Guid.NewGuid(), Quantity = 1 }, default);
        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(cart.Added);
    }

    [Fact]
    public async Task Remove_Delegates_To_Cart_Service()
    {
        var cart = new FakeCartService();
        var id = Guid.NewGuid();
        var result = Controller(cart).Remove(id, null);
        Assert.IsType<RedirectToActionResult>(result);
        Assert.Contains((id, (Guid?)null), cart.Removed);
    }
}

public class PaymentProviderTests
{
    [Fact]
    public async Task COD_Is_Not_Online_And_Stays_Pending()
    {
        IPaymentProvider p = new CashOnDeliveryProvider();
        Assert.False(p.IsOnline);
        Assert.Equal("Cash on Delivery", p.MethodName);
        var r = await p.CreateChargeAsync(1900m, "EGP");
        Assert.True(r.Ok);
        Assert.Equal(market_mvc.Domain.Enums.PaymentStatus.Pending, r.Status);
        Assert.Null(r.TransactionId);
    }

    [Fact]
    public async Task COD_Rejects_NonPositive_Amounts()
    {
        IPaymentProvider p = new CashOnDeliveryProvider();
        var r = await p.CreateChargeAsync(0m, "EGP");
        Assert.False(r.Ok);
    }
}

public class RateLimitingTests
{
    [Fact]
    public void PartitionKey_Normalizes_And_FailCloses()
    {
        Assert.Equal("unknown", RateLimiting.PartitionKey(null));
        Assert.Equal("unknown", RateLimiting.PartitionKey("  "));
        Assert.Equal("1.2.3.4", RateLimiting.PartitionKey("1.2.3.4"));
        Assert.Equal("::1", RateLimiting.PartitionKey("::1"));
    }
}

public class EmailSenderTests
{
    [Fact]
    public async Task FileSender_Writes_Eml_Without_Smtp()
    {
        var root = Path.Combine(Path.GetTempPath(), "zonta-mail-test", Guid.NewGuid().ToString("N"));
        var env = new TestHostEnvironment { ContentRootPath = root };
        var sender = new FileEmailSender(env, NullLogger<FileEmailSender>.Instance);
        Assert.False(sender.IsConfigured);
        await sender.SendAsync("a@example.com", "Subject", "<p>Hi</p>");
        Assert.Single(Directory.GetFiles(Path.Combine(root, "App_Data", "mail"), "*.eml"));
        Directory.Delete(root, true);
    }

    [Fact]
    public void SmtpSender_Reports_Unconfigured_Without_Host()
    {
        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build();
        var sender = new SmtpEmailSender(config, NullLogger<SmtpEmailSender>.Instance);
        Assert.False(sender.IsConfigured);
    }

    private sealed class TestHostEnvironment : IWebHostEnvironment
    {
        public string ContentRootPath { get; set; } = "";
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "Tests";
        public Microsoft.Extensions.FileProviders.IFileProvider? ContentRootFileProvider { get; set; }
        public Microsoft.Extensions.FileProviders.IFileProvider? WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; } = "";
    }
}

