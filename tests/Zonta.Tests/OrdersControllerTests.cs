using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using market_mvc.Controllers;
using market_mvc.Domain;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Features.Orders.Services;

namespace Zonta.Tests;

public class FakeTempDataProvider : ITempDataProvider
{
    public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
    public void SaveTempData(HttpContext context, IDictionary<string, object?> values) { }
}

public class FakeOrderService : IOrderService
{
    public List<Order> Orders { get; } = new();
    public List<(Guid Id, OrderStatus Status)> StatusUpdates { get; } = new();
    public bool ThrowInvalidTransition { get; set; }

    public Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct = default)
        => Task.FromResult<IEnumerable<Order>>(Orders);

    public Task<IEnumerable<Order>> GetOrdersForUserAsync(string email, CancellationToken ct = default)
        => Task.FromResult<IEnumerable<Order>>(Orders.Where(o =>
            string.Equals(o.CustomerEmail, email, StringComparison.OrdinalIgnoreCase)));

    public Task<PaginatedResult<Order>> GetOrdersAsync(OrderSearchRequest request, CancellationToken ct = default)
        => Task.FromResult(new PaginatedResult<Order> { Items = Orders });

    public Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(Orders.FirstOrDefault(o => o.Id == id));

    public Task<Order?> GetOrderForUserAsync(Guid id, string email, bool isPrivileged, CancellationToken ct = default)
    {
        var order = Orders.FirstOrDefault(o => o.Id == id);
        if (order == null) return Task.FromResult<Order?>(null);
        if (isPrivileged) return Task.FromResult<Order?>(order);
        return Task.FromResult<Order?>(string.Equals(order.CustomerEmail, email, StringComparison.OrdinalIgnoreCase) ? order : null);
    }

    public Task<(bool Ok, string? Error, Guid OrderId)> CreateOrderFromCartAsync(CheckoutInput input, CancellationToken ct = default)
        => Task.FromResult((true, (string?)null, Guid.NewGuid()));

    public Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
    {
        if (ThrowInvalidTransition) throw new InvalidOperationException("Cannot move order.");
        StatusUpdates.Add((id, status));
        return Task.CompletedTask;
    }

    public Task DeleteOrderAsync(Guid id, CancellationToken ct = default)
    {
        Orders.RemoveAll(o => o.Id == id);
        return Task.CompletedTask;
    }

#pragma warning disable CS0618
    public Task<Guid> CreateOrderAsync(decimal total) => throw new NotSupportedException();
#pragma warning restore CS0618

    public Task UpdateOrderAsync(Order order) => Task.CompletedTask;
}

public static class ClaimsHelper
{
    public static ClaimsPrincipal User(string email, params string[] roles)
    {
        var claims = new List<Claim> { new(ClaimTypes.Email, email), new(ClaimTypes.Name, email) };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
}

public class OrdersControllerTests
{
    private static Order NewOrder(string email) => new()
    {
        Id = Guid.NewGuid(),
        OrderNumber = "ZON-TEST",
        CustomerEmail = email,
        SubTotal = 100m,
        Total = 100m
    };

    private static OrdersController Controller(FakeOrderService svc, ClaimsPrincipal user)
    {
        var c = new OrdersController(svc)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } },
            TempData = new TempDataDictionary(new DefaultHttpContext(), new FakeTempDataProvider())
        };
        return c;
    }

    [Fact]
    public async Task Index_Customer_Sees_Only_Own_Orders()
    {
        var svc = new FakeOrderService();
        svc.Orders.Add(NewOrder("a@example.com"));
        svc.Orders.Add(NewOrder("b@example.com"));
        var result = await Controller(svc, ClaimsHelper.User("a@example.com")).Index(default);
        var view = Assert.IsType<ViewResult>(result);
        Assert.Single((IEnumerable<Order>)view.Model!);
    }

    [Fact]
    public async Task Index_Admin_Sees_All_Orders()
    {
        var svc = new FakeOrderService();
        svc.Orders.Add(NewOrder("a@example.com"));
        svc.Orders.Add(NewOrder("b@example.com"));
        var result = await Controller(svc, ClaimsHelper.User("admin@example.com", "Admin")).Index(default);
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(2, ((IEnumerable<Order>)view.Model!).Count());
    }

    [Fact]
    public async Task Details_Others_Order_Returns_NotFound()
    {
        var svc = new FakeOrderService();
        var order = NewOrder("a@example.com");
        svc.Orders.Add(order);
        var result = await Controller(svc, ClaimsHelper.User("b@example.com")).Details(order.Id, default);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_Privileged_Can_View_Others_Order()
    {
        var svc = new FakeOrderService();
        var order = NewOrder("a@example.com");
        svc.Orders.Add(order);
        var result = await Controller(svc, ClaimsHelper.User("s@example.com", "Support")).Details(order.Id, default);
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Edit_POST_Invalid_Transition_Shows_Error()
    {
        var svc = new FakeOrderService { ThrowInvalidTransition = true };
        var order = NewOrder("a@example.com");
        svc.Orders.Add(order);
        var c = Controller(svc, ClaimsHelper.User("admin@example.com", "Admin"));
        var result = await c.Edit(order.Id, new OrdersController.UpdateStatusInput(order.Id, OrderStatus.Delivered), default);
        Assert.IsType<ViewResult>(result);
        Assert.False(c.ModelState.IsValid);
    }
}

