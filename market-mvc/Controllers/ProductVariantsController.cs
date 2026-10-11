using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Infrastructure.Localization;
using market_mvc.Domain.entity;

namespace market_mvc.Controllers
{
    [Authorize(Policy = "AdminOrManager")]
    public class ProductVariantsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IZontaLocalizer _localizer;
        private string _L(string key) => _localizer.Get(key);

        public ProductVariantsController(AppDbContext db, IZontaLocalizer localizer)
        {
            _db = db;
            _localizer = localizer;
        }

        public class VariantInput
        {
            [Required]
            public Guid ProductId { get; set; }
            [StringLength(20)]
            public string? Size { get; set; }
            [StringLength(50)]
            public string? Color { get; set; }
            [StringLength(100)]
            public string? Sku { get; set; }
            [Range(0, int.MaxValue)]
            public int StockQuantity { get; set; }
            [Range(0.01, double.MaxValue)]
            public decimal? Price { get; set; }
        }

        // POST: /ProductVariants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VariantInput input, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(input.Size) && string.IsNullOrWhiteSpace(input.Color))
            {
                TempData["ErrorMessage"] = _L("Msg.SizeColorRequired");
                return RedirectToAction("Edit", "Products", new { id = input.ProductId });
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _L("Msg.InvalidVariant");
                return RedirectToAction("Edit", "Products", new { id = input.ProductId });
            }

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == input.ProductId && !p.IsDeleted, ct);
            if (product == null) return NotFound();

            // Prevent duplicate size/color combos (case-insensitive).
            var size = input.Size?.Trim();
            var color = input.Color?.Trim();
            var duplicate = await _db.ProductVariants.AnyAsync(v =>
                v.ProductId == input.ProductId && !v.IsDeleted &&
                (v.Size ?? "") == (size ?? "") && (v.Color ?? "") == (color ?? ""), ct);
            if (duplicate)
            {
                TempData["ErrorMessage"] = _L("Msg.VariantExists");
                return RedirectToAction("Edit", "Products", new { id = input.ProductId });
            }

            _db.ProductVariants.Add(new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = input.ProductId,
                Size = string.IsNullOrWhiteSpace(size) ? null : size,
                Color = string.IsNullOrWhiteSpace(color) ? null : color,
                Sku = string.IsNullOrWhiteSpace(input.Sku) ? null : input.Sku.Trim(),
                StockQuantity = input.StockQuantity,
                Price = input.Price,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);
            TempData["SuccessMessage"] = _L("Msg.VariantAdded");
            return RedirectToAction("Edit", "Products", new { id = input.ProductId });
        }

        // POST: /ProductVariants/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
            if (variant == null) return NotFound();

            // Refuse to delete variants referenced by existing order items (preserve history).
            var referenced = await _db.OrderItems.AnyAsync(o => o.ProductVariantId == id && !o.IsDeleted, ct);
            if (referenced)
            {
                variant.IsActive = false; // soft-deactivate instead
                TempData["SuccessMessage"] = _L("Msg.VariantDeactivated");
            }
            else
            {
                _db.ProductVariants.Remove(variant); // hard delete safe: no history
                TempData["SuccessMessage"] = _L("Msg.VariantDeleted");
            }
            await _db.SaveChangesAsync(ct);
            return RedirectToAction("Edit", "Products", new { id = variant.ProductId });
        }

        // POST: /ProductVariants/ToggleActive/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id, CancellationToken ct)
        {
            var variant = await _db.ProductVariants.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, ct);
            if (variant == null) return NotFound();
            variant.IsActive = !variant.IsActive;
            variant.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            TempData["SuccessMessage"] = variant.IsActive ? "Variant activated." : "Variant deactivated.";
            return RedirectToAction("Edit", "Products", new { id = variant.ProductId });
        }
    }
}

