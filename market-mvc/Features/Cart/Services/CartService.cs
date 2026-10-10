using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ViewModels.Cart;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Features.Cart.Services
{
    public interface ICartService
    {
        List<CartLineVM> GetLines();
        Task<CartVM> GetCartAsync(CancellationToken ct = default);
        Task<(bool Ok, string? Error)> AddAsync(Guid productId, Guid? variantId, int quantity, CancellationToken ct = default);
        Task<(bool Ok, string? Error)> UpdateQuantityAsync(Guid productId, Guid? variantId, int quantity, CancellationToken ct = default);
        void Remove(Guid productId, Guid? variantId);
        void Clear();
        int Count();
    }

    public class CartService : ICartService
    {
        private const string SessionKey = "zonta_cart_v1";
        private const int MaxQtyPerLine = 99;
        private readonly IHttpContextAccessor _http;
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public CartService(IHttpContextAccessor http, AppDbContext db, IConfiguration config)
        {
            _http = http;
            _db = db;
            _config = config;
        }

        public List<CartLineVM> GetLines()
        {
            var session = _http.HttpContext?.Session;
            if (session == null) return new();
            var json = session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(json)) return new();
            try
            {
                var lines = JsonSerializer.Deserialize<List<CartLineVM>>(json) ?? new();
                // Backward compat: old entries without VariantId deserialize as null.
                return lines;
            }
            catch
            {
                return new();
            }
        }

        private void SaveLines(List<CartLineVM> lines)
        {
            _http.HttpContext?.Session.SetString(SessionKey, JsonSerializer.Serialize(lines));
        }

        private static bool SameLine(CartLineVM l, Guid productId, Guid? variantId) =>
            l.ProductId == productId && l.VariantId == variantId;

        public async Task<CartVM> GetCartAsync(CancellationToken ct = default)
        {
            var lines = GetLines();
            var vm = new CartVM
            {
                ShippingAmount = 0m // computed below from verified free-shipping rule
            };
            if (!lines.Any()) return vm;

            var productIds = lines.Select(l => l.ProductId).Distinct().ToList();
            var products = await _db.Products
                .Include(p => p.Variants)
                .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
                .ToDictionaryAsync(p => p.Id, ct);

            foreach (var line in lines)
            {
                if (!products.TryGetValue(line.ProductId, out var p)) continue;
                if (p.Status != ProductStatus.Active) continue;

                decimal unitPrice = p.Price;
                string? variantLabel = null;
                string? sku = p.Sku;
                int stock = p.StockQuantity;
                bool available = p.Status == ProductStatus.Active;

                if (line.VariantId.HasValue)
                {
                    var v = p.Variants.FirstOrDefault(x => x.Id == line.VariantId.Value && !x.IsDeleted && x.IsActive);
                    if (v == null) continue; // variant removed/deactivated: drop line silently
                    unitPrice = v.EffectivePrice(p.Price);
                    variantLabel = v.Label;
                    sku = v.Sku ?? p.Sku;
                    stock = v.StockQuantity;
                    available = available && v.StockQuantity > 0;
                }
                else if (p.HasVariants)
                {
                    // Product now has variants but line has none: require explicit selection.
                    // Keep line but mark unavailable so checkout blocks with a clear message.
                    available = false;
                }
                else
                {
                    available = available && (!p.TrackQuantity || p.StockQuantity > 0);
                }

                vm.Items.Add(new CartItemVM
                {
                    ProductId = p.Id,
                    VariantId = line.VariantId,
                    ProductName = p.Name,
                    VariantLabel = variantLabel,
                    Sku = sku,
                    ImageUrl = p.ImageUrl,
                    ImageAlt = p.ImageAlt ?? p.Name,
                    UnitPrice = unitPrice, // authoritative server price
                    Quantity = Math.Min(line.Quantity, MaxQtyPerLine),
                    StockQuantity = stock,
                    IsAvailable = available
                });
            }

            vm.ShippingAmount = Features.Shipping.ZontaShipping.Compute(vm.SubTotal, _config).Amount;
            return vm;
        }

        public async Task<(bool Ok, string? Error)> AddAsync(Guid productId, Guid? variantId, int quantity, CancellationToken ct = default)
        {
            if (quantity < 1 || quantity > MaxQtyPerLine)
                return (false, "Quantity must be between 1 and 99.");

            var product = await _db.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted, ct);
            if (product == null) return (false, "Product not found.");
            if (product.Status != ProductStatus.Active) return (false, "Product is not available.");

            int availableStock;
            if (variantId.HasValue)
            {
                var v = product.Variants.FirstOrDefault(x => x.Id == variantId.Value && !x.IsDeleted && x.IsActive);
                if (v == null) return (false, "Selected size/color is not available.");
                availableStock = v.StockQuantity;
                if (availableStock <= 0) return (false, $"This size/color ({v.Label}) is out of stock.");
            }
            else
            {
                if (product.HasVariants) return (false, "Please select a size/color.");
                if (product.TrackQuantity && product.StockQuantity <= 0) return (false, "Product is out of stock.");
                availableStock = product.StockQuantity;
            }

            var lines = GetLines();
            var existing = lines.FirstOrDefault(l => SameLine(l, productId, variantId));
            var newQty = (existing?.Quantity ?? 0) + quantity;
            if (newQty > MaxQtyPerLine) return (false, "Maximum 99 units per line.");
            if (variantId.HasValue || product.TrackQuantity)
            {
                if (newQty > availableStock)
                    return (false, $"Only {availableStock} unit(s) available.");
            }

            if (existing == null) lines.Add(new CartLineVM { ProductId = productId, VariantId = variantId, Quantity = quantity });
            else existing.Quantity = newQty;

            SaveLines(lines);
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> UpdateQuantityAsync(Guid productId, Guid? variantId, int quantity, CancellationToken ct = default)
        {
            if (quantity < 0 || quantity > MaxQtyPerLine)
                return (false, "Quantity must be between 0 and 99.");

            var lines = GetLines();
            var existing = lines.FirstOrDefault(l => SameLine(l, productId, variantId));
            if (existing == null) return (false, "Item not in cart.");

            if (quantity == 0)
            {
                lines.Remove(existing);
                SaveLines(lines);
                return (true, null);
            }

            var product = await _db.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted, ct);
            if (product == null) return (false, "Product not found.");

            int stock;
            if (variantId.HasValue)
            {
                var v = product.Variants.FirstOrDefault(x => x.Id == variantId.Value && !x.IsDeleted && x.IsActive);
                if (v == null) return (false, "Selected size/color is not available.");
                stock = v.StockQuantity;
            }
            else
            {
                if (product.HasVariants) return (false, "Please select a size/color.");
                stock = product.StockQuantity;
            }

            if ((variantId.HasValue || product.TrackQuantity) && quantity > stock)
                return (false, $"Only {stock} unit(s) available.");

            existing.Quantity = quantity;
            SaveLines(lines);
            return (true, null);
        }

        public void Remove(Guid productId, Guid? variantId)
        {
            var lines = GetLines();
            lines.RemoveAll(l => SameLine(l, productId, variantId));
            SaveLines(lines);
        }

        public void Clear() => SaveLines(new());

        public int Count() => GetLines().Sum(l => l.Quantity);
    }
}
