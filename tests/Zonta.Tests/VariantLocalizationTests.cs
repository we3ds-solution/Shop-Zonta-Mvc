using Microsoft.AspNetCore.Http;
using market_mvc.Domain.entity;
using market_mvc.Domain.ViewModels.Cart;
using market_mvc.Infrastructure.Localization;

namespace Zonta.Tests;

public class VariantRulesTests
{
    [Fact]
    public void Variant_Label_JoinsSizeAndColor()
    {
        Assert.Equal("EU 42 / Black", new ProductVariant { Size = "EU 42", Color = "Black" }.Label);
        Assert.Equal("EU 42", new ProductVariant { Size = "EU 42" }.Label);
        Assert.Equal("Standard", new ProductVariant().Label);
    }

    [Fact]
    public void Variant_EffectivePrice_FallsBackToProductPrice()
    {
        Assert.Equal(1000m, new ProductVariant { Price = null }.EffectivePrice(1000m));
        Assert.Equal(1200m, new ProductVariant { Price = 1200m }.EffectivePrice(1000m));
    }

    [Fact]
    public void Variant_CanFulfill_RespectsStock()
    {
        var v = new ProductVariant { StockQuantity = 2 };
        Assert.True(v.CanFulfill(2));
        Assert.False(v.CanFulfill(3));
    }

    [Fact]
    public void CartItem_DisplayName_IncludesVariantLabel()
    {
        var item = new CartItemVM { ProductName = "Hob", VariantLabel = "EU 42 / Black", UnitPrice = 100m, Quantity = 2 };
        Assert.Equal("Hob (EU 42 / Black)", item.DisplayName);
        Assert.Equal(200m, item.LineTotal);
    }

    [Fact]
    public void Product_HasVariants_False_When_None()
    {
        Assert.False(new Product { Name = "X", Price = 10m }.HasVariants);
    }

    [Fact]
    public void Product_HasVariants_True_When_ActiveVariant()
    {
        var p = new Product { Name = "X", Price = 10m };
        p.Variants.Add(new ProductVariant { Size = "42", IsActive = true });
        Assert.True(p.HasVariants);
    }
}

public class LocalizationTests
{
    private static IZontaLocalizer EnLocalizer()
    {
        var ctx = new DefaultHttpContext();
        return new ZontaLocalizer(new HttpContextAccessor { HttpContext = ctx });
    }

    [Fact]
    public void Localizer_Defaults_To_English_LTR()
    {
        var l = EnLocalizer();
        Assert.Equal("en", l.Culture);
        Assert.False(l.IsRtl);
        Assert.Equal("Home", l.Get("Nav.Home"));
        Assert.Equal("EGP", l.CurrencyLabel);
    }

    [Fact]
    public void Localizer_Unknown_Key_Returns_Key()
    {
        Assert.Equal("Nope.Missing", EnLocalizer().Get("Nope.Missing"));
    }

    [Fact]
    public void Localizer_Arabic_Cookie_Switches_RTL()
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Headers.Cookie = "zonta_culture=ar";
        var l = new ZontaLocalizer(new HttpContextAccessor { HttpContext = ctx });
        Assert.Equal("ar", l.Culture);
        Assert.True(l.IsRtl);
        Assert.Equal("السلة", l.Get("Nav.Cart"));
        Assert.Equal("ج.م", l.CurrencyLabel);
    }
}
