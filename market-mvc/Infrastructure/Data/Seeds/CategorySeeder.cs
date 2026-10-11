using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// Seeds the Zonta Egypt structural catalog (categories only, no products).
    /// Product data requires verified official import (shopzonta.com was inaccessible
    /// on 2026-10-10). Never seeds fabricated prices or stock. Idempotent by name.
    /// </summary>
    public class CategorySeeder : ISeeder
    {
        public string Name => "Categories";
        public int Order => 1;

        public async Task SeedAsync(DbContext context)
        {
            var db = context as AppDbContext
                ?? throw new InvalidOperationException("Context must be AppDbContext");

            var zontaCategories = new List<(string Name, string Description, CategoryType Type, string Slug, int Sort, string Image, string Alt)>
            {
                ("Sneakers", "Zonta Arabic sneakers — premium streetwear footwear.", CategoryType.Footwear, "sneakers", 1, "/images/categories/footwear/habibi-sneakers.jpg", "Habibi Arabic sneakers by Zonta"),
                ("Apparel", "Zonta streetwear apparel.", CategoryType.Apparel, "apparel", 2, "/images/categories/apparel/lifestyle-brunch.jpg", "Zonta streetwear lifestyle"),
                ("Accessories", "Zonta accessories.", CategoryType.Accessories, "accessories", 3, "/images/categories/accessories/wear-language.jpg", "Wear your language — Zonta accessories"),
                ("Collections", "Zonta drops and curated collections (e.g. Drop 1.0).", CategoryType.Collections, "collections", 4, "/images/categories/collections/arab-heritage.jpg", "Born in the Arab world, worn around the globe"),
            };

            foreach (var (name, desc, type, slug, sort, image, alt) in zontaCategories)
            {
                var existing = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
                if (existing == null)
                {
                    db.Categories.Add(new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Description = desc,
                        Type = type,
                        Slug = slug,
                        IsActive = true,
                        SortOrder = sort,
                        ImageUrl = image,
                        ImageAlt = alt,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "Seeder"
                    });
                }
                else
                {
                    // Backfill Instagram brand images onto existing rows (idempotent).
                    if (string.IsNullOrWhiteSpace(existing.ImageUrl))
                    {
                        existing.ImageUrl = image;
                        existing.ImageAlt = alt;
                    }
                    if (!existing.IsActive)
                    {
                        existing.IsActive = true;
                        existing.Type = type;
                    }
                    if (string.IsNullOrWhiteSpace(existing.Slug)) existing.Slug = slug;
                }
            }

            // Deactivate legacy demo categories from the old generic marketplace.
            var legacyNames = new[] { "Electronics", "Clothing", "Books", "Home & Garden", "Sports & Outdoors" };
            var legacy = await db.Categories.Where(c => legacyNames.Contains(c.Name)).ToListAsync();
            foreach (var cat in legacy)
            {
                cat.IsActive = false;
            }

            await db.SaveChangesAsync();
        }
    }
}
