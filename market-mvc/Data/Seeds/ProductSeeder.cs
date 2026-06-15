using Microsoft.EntityFrameworkCore;
using market_mvc.Data;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;

namespace market_mvc.Data.Seeds
{
    /// <summary>
    /// Seeder for product data
    /// </summary>
    public class ProductSeeder : ISeeder
    {
        public string Name => "Products";
        public int Order => 3; // Execute third (after categories)

        public async Task SeedAsync(DbContext context)
        {
            var appDbContext = context as AppDbContext;
            if (appDbContext == null)
                throw new InvalidOperationException("Context must be AppDbContext");

            if (await appDbContext.Products.AnyAsync())
            {
                return; // Products already seeded
            }

            var categories = await appDbContext.Categories.ToListAsync();

            var electronicsCategory = categories.FirstOrDefault(c => c.Type == CategoryType.Electronics);
            var clothingCategory = categories.FirstOrDefault(c => c.Type == CategoryType.Clothing);
            var booksCategory = categories.FirstOrDefault(c => c.Type == CategoryType.Books);
            var homeCategory = categories.FirstOrDefault(c => c.Type == CategoryType.Home);
            var sportsCategory = categories.FirstOrDefault(c => c.Type == CategoryType.Sports);

            var products = new List<Product>
            {
                CreateProduct(
                    name: "Wireless Bluetooth Headphones",
                    description: "High-quality wireless headphones with noise cancellation and 30-hour battery life.",
                    price: 199.99m,
                    compareAtPrice: 249.99m,
                    costPrice: 120.00m,
                    imageUrl: "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400",
                    imageAlt: "Wireless Bluetooth Headphones",
                    sku: "WBH-001",
                    barcode: "1234567890123",
                    stockQuantity: 50,
                    categoryId: electronicsCategory?.Id,
                    sortOrder: 1,
                    isFeatured: true
                ),
                CreateProduct(
                    name: "Cotton T-Shirt",
                    description: "Comfortable 100% cotton t-shirt available in multiple colors and sizes.",
                    price: 24.99m,
                    compareAtPrice: 34.99m,
                    costPrice: 12.00m,
                    imageUrl: "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400",
                    imageAlt: "Cotton T-Shirt",
                    sku: "CTS-001",
                    barcode: "1234567890124",
                    stockQuantity: 100,
                    categoryId: clothingCategory?.Id,
                    sortOrder: 2,
                    isFeatured: false
                ),
                CreateProduct(
                    name: "Programming Fundamentals Book",
                    description: "Comprehensive guide to programming fundamentals for beginners and intermediate developers.",
                    price: 49.99m,
                    compareAtPrice: 59.99m,
                    costPrice: 25.00m,
                    imageUrl: "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=400",
                    imageAlt: "Programming Fundamentals Book",
                    sku: "PFB-001",
                    barcode: "1234567890125",
                    stockQuantity: 30,
                    categoryId: booksCategory?.Id,
                    sortOrder: 3,
                    isFeatured: true
                ),
                CreateProduct(
                    name: "Smart Home Security Camera",
                    description: "WiFi-enabled security camera with night vision, motion detection, and mobile app control.",
                    price: 129.99m,
                    compareAtPrice: 179.99m,
                    costPrice: 80.00m,
                    imageUrl: "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400",
                    imageAlt: "Smart Home Security Camera",
                    sku: "SHSC-001",
                    barcode: "1234567890126",
                    stockQuantity: 25,
                    categoryId: homeCategory?.Id,
                    sortOrder: 4,
                    isFeatured: true
                ),
                CreateProduct(
                    name: "Yoga Mat Premium",
                    description: "Non-slip premium yoga mat with extra cushioning and carrying strap.",
                    price: 39.99m,
                    compareAtPrice: 54.99m,
                    costPrice: 20.00m,
                    imageUrl: "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=400",
                    imageAlt: "Premium Yoga Mat",
                    sku: "YMP-001",
                    barcode: "1234567890127",
                    stockQuantity: 40,
                    categoryId: sportsCategory?.Id,
                    sortOrder: 5,
                    isFeatured: false
                )
            };

            await appDbContext.Products.AddRangeAsync(products);
            await appDbContext.SaveChangesAsync();
        }

        private static Product CreateProduct(
            string name,
            string description,
            decimal price,
            decimal compareAtPrice,
            decimal costPrice,
            string imageUrl,
            string imageAlt,
            string sku,
            string barcode,
            int stockQuantity,
            Guid? categoryId,
            int sortOrder,
            bool isFeatured)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Price = price,
                CompareAtPrice = compareAtPrice,
                CostPrice = costPrice,
                ImageUrl = imageUrl,
                ImageAlt = imageAlt,
                Sku = sku,
                Barcode = barcode,
                StockQuantity = stockQuantity,
                LowStockThreshold = stockQuantity / 5,
                TrackQuantity = true,
                Weight = 0.5m,
                WeightUnit = "kg",
                Status = ProductStatus.Active,
                IsFeatured = isFeatured,
                SortOrder = sortOrder,
                MetaTitle = $"Buy {name} Online",
                MetaDescription = description[..Math.Min(160, description.Length)],
                Tags = name.ToLower().Replace(" ", ","),
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Seeder"
            };
        }
    }
}


