using market_mvc.Domain.entity;
using market_mvc.Domain.ObjectValues;
using market_mvc.Domain.ViewModels.Cart;

namespace Zonta.Tests;

public class CommerceRulesTests
{
    [Fact]
    public void Order_CalculateTotal_SumsComponents()
    {
        var order = new Order { SubTotal = 2000m, TaxAmount = 0m, ShippingAmount = 60m, DiscountAmount = 100m };
        order.CalculateTotal();
        Assert.Equal(1960m, order.Total);
    }

    [Fact]
    public void Cart_Totals_SumServerLineTotals()
    {
        var cart = new CartVM { ShippingAmount = 60m };
        cart.Items.Add(new CartItemVM { ProductId = Guid.NewGuid(), ProductName = "Hob", UnitPrice = 1000m, Quantity = 2, StockQuantity = 5, IsAvailable = true });
        cart.Items.Add(new CartItemVM { ProductId = Guid.NewGuid(), ProductName = "Fakhr", UnitPrice = 500m, Quantity = 1, StockQuantity = 5, IsAvailable = true });
        Assert.Equal(2500m, cart.SubTotal);
        Assert.Equal(2560m, cart.Total);
    }

    [Fact]
    public void Product_CanFulfillOrder_RespectsStock()
    {
        var p = new Product { Name = "Sneaker", Price = 1000m, StockQuantity = 2, TrackQuantity = true };
        Assert.True(p.CanFulfillOrder(2));
        Assert.False(p.CanFulfillOrder(3));
    }

    [Fact]
    public void Money_Defaults_To_EGP()
    {
        Assert.Equal("EGP", new Money().Currency);
        Assert.Equal("EGP", new Money(10m).Currency);
    }

    [Fact]
    public void OrderItem_LineTotal_IsQuantityTimesPrice()
    {
        var item = new OrderItem { Quantity = 3, UnitPrice = 1500m };
        Assert.Equal(4500m, item.LineTotal);
    }
}
