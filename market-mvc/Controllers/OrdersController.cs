using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using market_mvc.Domain;
using market_mvc.Domain.entity;
using market_mvc.Domain.ViewModels.Order;
using market_mvc.Features.Orders.Services;

namespace market_mvc.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View(new CreateOrderVM());
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var orderId = await _orderService.CreateOrderAsync(model.Total);

            model.OrderId = orderId;
            ModelState.Clear();

            return View(model);
        }

        // GET: Orders/Edit/5
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        [Authorize(Policy = "AdminOrManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderAsync(order);
                TempData["SuccessMessage"] = "Order updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [Authorize(Policy = "AdminOrManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _orderService.DeleteOrderAsync(id);
            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

