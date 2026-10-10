using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Infrastructure.Data;

namespace market_mvc.Features.Catalog
{
    public record CatalogImportItem(
        string Sku,
        string Name,
        string? Description,
        decimal Price,
        decimal? CompareAtPrice,
        string? ImageUrl,
        string? ImageAlt,
        string? Category,
        string? Availability,
        List<string>? Sizes,
        List<string>? Colors,
        string? SourceUrl,
        string? DateChecked,
        string? Status,
        bool? TrackQuantity);

    public record CatalogImportResult(int Created, int Updated, int VariantsCreated, List<string> Errors);

    public interface ICatalogImportService
    {
        Task<(bool Ok, string? Error, CatalogImportResult? Result)> ImportAsync(string filePath, CancellationToken ct = default);
    }

    /// <summary>
    /// Manual verified-catalog import. Idempotent by Sku (no duplicates).
    /// Never touches manually maintained stock levels or order data: on update,
    /// stock quantities are preserved; new variants are created inactive with
    /// zero stock so an admin must confirm availability before sale.
    /// </summary>
    public class CatalogImportService : ICatalogImportService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CatalogImportService> _logger;

        public CatalogImportService(AppDbContext db, ILogger<CatalogImportService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<(bool Ok, string? Error, CatalogImportResult? Result)> ImportAsync(string filePath, CancellationToken ct = default)
        {
            if (!File.Exists(filePath)) return (false, $"File not found: {filePath}", null);

            List<CatalogImportItem> items;
            try
            {
                var json = await File.ReadAllTextAsync(filePath, ct);
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("products", out var arr) || arr.ValueKind != JsonValueKind.Array)
                    return (false, "Invalid catalog file: missing 'products' array.", null);
                items = new List<CatalogImportItem>();
                foreach (var el in arr.EnumerateArray())
                {
                    string Get(string name) => el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() ?? "" : "";
                    decimal? GetDec(string name) => el.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.Number ? p.GetDecimal() : null;
                    List<string>? GetList(string name)
                    {
                        if (!el.TryGetProperty(name, out var p) || p.ValueKind != JsonValueKind.Array) return null;
                        return p.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!.Trim()).Where(s => s.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).Take(30).ToList();
                    }
                    var sku = Get("sku").Trim();
                    var name = Get("name").Trim();
                    if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name)) continue;
                    if (!el.TryGetProperty("price", out var priceEl) || priceEl.ValueKind != JsonValueKind.Number) continue;
                    items.Add(new CatalogImportItem(
                        sku, name,
                        el.TryGetProperty("description", out var d) && d.ValueKind == JsonValueKind.String ? d.GetString() : null,
                        priceEl.GetDecimal(),
                        GetDec("compareAtPrice"),
                        el.TryGetProperty("imageUrl", out var iu) && iu.ValueKind == JsonValueKind.String ? iu.GetString() : null,
                        el.TryGetProperty("imageAlt", out var ia) && ia.ValueKind == JsonValueKind.String ? ia.GetString() : null,
                        el.TryGetProperty("category", out var c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null,
                        el.TryGetProperty("availability", out var a) && a.ValueKind == JsonValueKind.String ? a.GetString() : null,
                        GetList("sizes"), GetList("colors"),
                        el.TryGetProperty("sourceUrl", out var s) && s.ValueKind == JsonValueKind.String ? s.GetString() : null,
                        el.TryGetProperty("dateChecked", out var dc) && dc.ValueKind == JsonValueKind.String ? dc.GetString() : null,
                        el.TryGetProperty("status", out var st) && st.ValueKind == JsonValueKind.String ? st.GetString() : null,
                        el.TryGetProperty("trackQuantity", out var tq) && (tq.ValueKind == JsonValueKind.True || tq.ValueKind == JsonValueKind.False) ? tq.GetBoolean() : null));
                }
            }
            catch (Exception ex)
            {
                return (false, $"Invalid catalog JSON: {ex.Message}", null);
            }

            var created = 0; var updated = 0; var variantsCreated = 0;
            var errors = new List<string>();

            var categories = await _db.Categories.Where(c => !c.IsDeleted).ToListAsync(ct);

            foreach (var item in items)
            {
                try
                {
                    if (item.Price <= 0) { errors.Add($"{item.Sku}: price must be positive."); continue; }
                    if (item.CompareAtPrice.HasValue && item.CompareAtPrice <= item.Price) { errors.Add($"{item.Sku}: compareAtPrice must exceed price."); continue; }

                    var category = await ResolveCategoryAsync(item.Category, categories, ct);
                    var inStock = !string.Equals(item.Availability, "out_of_stock", StringComparison.OrdinalIgnoreCase);

                    // Status honesty: Active only when explicitly verified ("listed" =
                    // shown for sale on the official store). Otherwise Draft for admin review.
                    var status = string.Equals(item.Status, "Active", StringComparison.OrdinalIgnoreCase) && inStock
                        ? ProductStatus.Active
                        : ProductStatus.Draft;
                    // Stock honesty: counts are never invented. trackQuantity=false means
                    // the store shows "Available" without quantity claims.
                    var trackQuantity = item.TrackQuantity ?? true;

                    var existing = await _db.Products
                        .Include(p => p.Variants)
                        .FirstOrDefaultAsync(p => p.Sku == item.Sku && !p.IsDeleted, ct);

                    if (existing == null)
                    {
                        var product = new Product
                        {
                            Id = Guid.NewGuid(),
                            Name = item.Name[..Math.Min(200, item.Name.Length)],
                            Description = (item.Description ?? "") is var desc && desc.Length > 2000 ? desc[..2000] : (desc ?? ""),
                            Price = item.Price,
                            CompareAtPrice = item.CompareAtPrice,
                            ImageUrl = item.ImageUrl,
                            ImageAlt = item.ImageAlt ?? item.Name,
                            Sku = item.Sku,
                            StockQuantity = 0, // never invent stock; admin sets after verification
                            TrackQuantity = trackQuantity,
                            Status = status,
                            CategoryId = category?.Id,
                            Tags = $"zonta,imported,{item.Sku.ToLowerInvariant()}",
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "CatalogImport"
                        };
                        _db.Products.Add(product);
                        await _db.SaveChangesAsync(ct);

                        variantsCreated += await EnsureVariantsAsync(product, item, ct);
                        created++;
                        _logger.LogInformation("Imported product {Sku}", item.Sku);
                    }
                    else
                    {
                        // Update descriptive fields only; preserve manual stock + order history.
                        existing.Name = item.Name[..Math.Min(200, item.Name.Length)];
                        if (item.Description != null)
                            existing.Description = item.Description.Length > 2000 ? item.Description[..2000] : item.Description;
                        existing.Price = item.Price;
                        existing.CompareAtPrice = item.CompareAtPrice;
                        if (item.ImageUrl != null) existing.ImageUrl = item.ImageUrl;
                        if (item.ImageAlt != null) existing.ImageAlt = item.ImageAlt;
                        if (category != null) existing.CategoryId = category.Id;
                        existing.UpdatedAt = DateTime.UtcNow;
                        await _db.SaveChangesAsync(ct);

                        variantsCreated += await EnsureVariantsAsync(existing, item, ct);
                        updated++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{item.Sku}: {ex.Message}");
                }
            }

            return (true, errors.Any() ? string.Join("; ", errors) : null,
                new CatalogImportResult(created, updated, variantsCreated, errors));
        }

        private async Task<Category?> ResolveCategoryAsync(string? name, List<Category> categories, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name)) return categories.FirstOrDefault(c => c.Name == "Sneakers");
            var found = categories.FirstOrDefault(c => string.Equals(c.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
            if (found != null) return found;

            var type = name.Trim().ToLowerInvariant() switch
            {
                var n when n.Contains("sneaker") || n.Contains("shoe") || n.Contains("footwear") => CategoryType.Footwear,
                var n when n.Contains("apparel") || n.Contains("cloth") || n.Contains("shirt") || n.Contains("hoodie") => CategoryType.Apparel,
                var n when n.Contains("accessor") || n.Contains("cap") || n.Contains("bag") => CategoryType.Accessories,
                _ => CategoryType.Collections
            };
            var cat = new Category
            {
                Id = Guid.NewGuid(),
                Name = name.Trim()[..Math.Min(200, name.Trim().Length)],
                Description = "Imported Zonta category (verify).",
                Type = type,
                Slug = name.Trim().ToLowerInvariant().Replace(' ', '-'),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "CatalogImport"
            };
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync(ct);
            categories.Add(cat);
            return cat;
        }

        private async Task<int> EnsureVariantsAsync(Product product, CatalogImportItem item, CancellationToken ct)
        {
            var sizes = item.Sizes is { Count: > 0 } ? item.Sizes : new List<string> { "" };
            var colors = item.Colors is { Count: > 0 } ? item.Colors : new List<string> { "" };
            if (sizes.Count == 1 && colors.Count == 1 && sizes[0] == "" && colors[0] == "") return 0;

            var added = 0;
            var existing = await _db.ProductVariants
                .Where(v => v.ProductId == product.Id && !v.IsDeleted)
                .ToListAsync(ct);

            foreach (var s in sizes)
            {
                foreach (var c in colors)
                {
                    var size = string.IsNullOrWhiteSpace(s) ? null : s.Trim();
                    var color = string.IsNullOrWhiteSpace(c) ? null : c.Trim();
                    if (size == null && color == null) continue;
                    var dup = existing.Any(v => (v.Size ?? "") == (size ?? "") && (v.Color ?? "") == (color ?? ""));
                    if (dup) continue;
                    _db.ProductVariants.Add(new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Size = size,
                        Color = color,
                        Sku = $"{product.Sku}-{size ?? "OS"}-{color ?? "NA"}".Replace(' ', '-'),
                        StockQuantity = 0, // admin confirms stock before activation
                        IsActive = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "CatalogImport"
                    });
                    added++;
                }
            }
            if (added > 0) await _db.SaveChangesAsync(ct);
            return added;
        }
    }
}
