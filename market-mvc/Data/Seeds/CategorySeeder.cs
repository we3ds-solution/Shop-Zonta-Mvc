using Microsoft.EntityFrameworkCore;
using market_mvc.Data;
using market_mvc.Models.entity;
using market_mvc.Models.Enums;

namespace market_mvc.Data.Seeds
{
    /// <summary>
    /// Seeder for product categories
    /// </summary>
    public class CategorySeeder : ISeeder
    {
        public string Name => "Categories";
        public int Order => 1; // Execute first

        public async Task SeedAsync(DbContext context)
        {
            var appDbContext = context as AppDbContext;
            if (appDbContext == null)
                throw new InvalidOperationException("Context must be AppDbContext");

            if (await appDbContext.Categories.AnyAsync())
            {
                return; // Categories already seeded
            }

            var categories = new List<Category>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    Type = CategoryType.Electronics,
                    ImageUrl = "https://images.unsplash.com/photo-1498049794561-7780e7231661?w=400",
                    ImageAlt = "Electronics",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Clothing",
                    Description = "Fashion and apparel",
                    Type = CategoryType.Clothing,
                    ImageUrl = "https://images.unsplash.com/photo-1441986300917-64674bd600d8?w=400",
                    ImageAlt = "Clothing",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Books",
                    Description = "Books and literature",
                    Type = CategoryType.Books,
                    ImageUrl = "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=400",
                    ImageAlt = "Books",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Home & Garden",
                    Description = "Home improvement and garden supplies",
                    Type = CategoryType.Home,
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=400",
                    ImageAlt = "Home & Garden",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Sports & Outdoors",
                    Description = "Sports equipment and outdoor gear",
                    Type = CategoryType.Sports,
                    ImageUrl = "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=400",
                    ImageAlt = "Sports & Outdoors",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Seeder"
                }
            };

            await appDbContext.Categories.AddRangeAsync(categories);
            await appDbContext.SaveChangesAsync();
        }
    }
}

