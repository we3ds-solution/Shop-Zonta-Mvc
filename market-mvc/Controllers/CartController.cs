using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using market_mvc.Domain.ViewModels.Cart;
using market_mvc.Features.Cart.Services;
using market_mvc.Features.Orders.Services;

namespace market_mvc.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cart;
        private readonly IOrderService _orders;
        private readonly IConfiguration _config;

        public CartController(ICartService cart, IOrderService orders, IConfiguration config)
        {
            _cart = cart;
            _orders = orders;
            _config = config;
        }

        // GET: /Cart
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var cart = await _cart.GetCartAsync(ct);
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddToCartVM model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid quantity.";
                return RedirectToAction(nameof(Index));
            }

            var (ok, error) = await _cart.AddAsync(model.ProductId, model.VariantId, model.Quantity, ct);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Added to cart." : error;
            if (!ok) return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid productId, Guid? variantId, int quantity, CancellationToken ct)
        {
            var (ok, error) = await _cart.UpdateQuantityAsync(productId, variantId, quantity, ct);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] = ok ? "Cart updated." : error;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Guid productId, Guid? variantId)
        {
            _cart.Remove(productId, variantId);
            TempData["SuccessMessage"] = "Item removed.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Cart/Checkout (auth required so orders are owned)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout(CancellationToken ct)
        {
            var cart = await _cart.GetCartAsync(ct);
            if (cart.IsEmpty)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            var email = User.Identity?.Name ?? string.Empty;
            // Prefer email claim; fallback to Name (UserName may be email).
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
                ?? (email.Contains('@') ? email : string.Empty);

            return View(new CheckoutVM
            {
                Email = userEmail,
                Country = "Egypt",
                Cart = cart
            });
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        public async Task<IActionResult> Checkout(CheckoutVM model, CancellationToken ct)
        {
            var cart = await _cart.GetCartAsync(ct);
            model.Cart = cart;

            if (cart.IsEmpty)
            {
                ModelState.AddModelError(string.Empty, "Your cart is empty.");
                return View(model);
            }

            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var input = new CheckoutInput(
                userId,
                model.Email.Trim(),
                model.FirstName.Trim(),
                model.LastName.Trim(),
                string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
                model.Street.Trim(), model.City.Trim(), model.State.Trim(),
                model.ZipCode.Trim(), model.Country.Trim(),
                string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim());

            try
            {
                var (ok, error, orderId) = await _orders.CreateOrderFromCartAsync(input, ct);
                if (!ok)
                {
                    ModelState.AddModelError(string.Empty, error ?? "Checkout failed.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Order placed. Cash on delivery — payment pending.";
                return RedirectToAction("Confirmation", new { id = orderId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Checkout failed. Please try again.");
                return View(model);
            }
        }

        // GET: /Cart/Confirmation/{id} (owner only)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Confirmation(Guid id, CancellationToken ct)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.Identity?.Name ?? string.Empty;
            var privileged = User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Support");
            var order = await _orders.GetOrderForUserAsync(id, email, privileged, ct);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
