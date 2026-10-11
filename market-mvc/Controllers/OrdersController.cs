using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using market_mvc.Domain.Enums;
using market_mvc.Features.Orders.Services;
using market_mvc.Infrastructure.Localization;

namespace market_mvc.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IZontaLocalizer _localizer;
        private string _L(string key) => _localizer.Get(key);

        public OrdersController(IOrderService orderService, IZontaLocalizer localizer)
        {
            _orderService = orderService;
            _localizer = localizer;
        }

        // GET: Orders (scoped: customers see only their own)
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Support"))
            {
                var orders = await _orderService.GetAllOrdersAsync(ct);
                return View(orders);
            }

            var email = UserEmail();
            var mine = await _orderService.GetOrdersForUserAsync(email, ct);
            return View(mine);
        }

        // GET: Orders/Details/5 (owner or privileged only; no existence oracle)
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var order = await _orderService.GetOrderForUserAsync(id, UserEmail(), IsPrivileged(), ct);
            if (order == null) return NotFound();
            return View(order);
        }

        // Legacy insecure total-based creation removed. Redirect to real checkout.
        [HttpGet]
        public IActionResult Create() => RedirectToAction("Checkout", "Cart");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(object? _ = null) => RedirectToAction("Checkout", "Cart");

        // GET: Orders/Edit/5 (status updates only, admin/manager)
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var order = await _orderService.GetOrderByIdAsync(id, ct);
            if (order == null) return NotFound();
            return View(order);
        }

        public record UpdateStatusInput(Guid Id, OrderStatus Status);

        // POST: Orders/Edit/5 — status transition only (prevents overposting of totals/payment).
        [Authorize(Policy = "AdminOrManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateStatusInput input, CancellationToken ct)
        {
            if (id != input.Id) return NotFound();
            if (!Enum.IsDefined(input.Status)) ModelState.AddModelError(nameof(input.Status), "Invalid status.");

            if (!ModelState.IsValid)
            {
                var order = await _orderService.GetOrderByIdAsync(id, ct);
                if (order == null) return NotFound();
                return View(order);
            }

            try
            {
                await _orderService.UpdateStatusAsync(id, input.Status, ct);
                TempData["SuccessMessage"] = _L("Msg.StatusUpdated");
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var order = await _orderService.GetOrderByIdAsync(id, ct);
                if (order == null) return NotFound();
                return View(order);
            }
        }

        // POST: Orders/Delete/5
        [Authorize(Policy = "AdminOrManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _orderService.DeleteOrderAsync(id, ct);
            TempData["SuccessMessage"] = _L("Msg.OrderDeleted");
            return RedirectToAction(nameof(Index));
        }

        private string UserEmail() =>
            User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
            ?? User.Identity?.Name ?? string.Empty;

        private bool IsPrivileged() =>
            User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Support");
    }
}

