using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using market_mvc.Domain.entity;
using market_mvc.Domain.Enums;
using market_mvc.Infrastructure.Data;

namespace Zonta.Tests;

public class ZontaWebFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(
        Path.GetTempPath(), "zonta-tests", $"test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var json = """
                {
                  "Zonta": { "RateLimit": { "PermitLimit": 1000 }, "FreeShippingThreshold": 750, "ShippingFee": 0 },
                  "Database": { "Provider": "Sqlite" },
                  "ConnectionStrings": { "DefaultConnection": "Data Source: __DB__" }
                }
                """.Replace("__DB__", _dbPath.Replace("\\", "\\\\"));
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            config.AddJsonStream(new MemoryStream(bytes));
        });
        builder.ConfigureServices(services =>
        {
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { /* best effort */ }
    }

    public static string AntiforgeryToken(string html)
    {
        var m = Regex.Match(html,
            "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        if (!m.Success) throw new InvalidOperationException("Antiforgery token not found.");
        return m.Groups[1].Value;
    }

    public async Task<(HttpClient Client, string Email)> RegisterAsync(
        HttpClient? shared = null, string? password = null)
    {
        var client = shared ?? CreateClient();
        var email = $"t{Guid.NewGuid():N}@example.com";
        var get = await client.GetAsync("/Auth/Register");
        get.EnsureSuccessStatusCode();
        var token = AntiforgeryToken(await get.Content.ReadAsStringAsync());
        var post = await client.PostAsync("/Auth/Register", new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["FullName"] = "Test User",
                ["Email"] = email,
                ["Password"] = password ?? "TestPass123!",
                ["ConfirmPassword"] = password ?? "TestPass123!",
                ["AgreeToTerms"] = "true",
                ["__RequestVerificationToken"] = token
            }));
        if (post.StatusCode != HttpStatusCode.Redirect && post.StatusCode != HttpStatusCode.OK)
            throw new InvalidOperationException("Registration failed: " + (int)post.StatusCode);
        return (client, email);
    }

    public async Task<Guid> SeedProductAsync(decimal price = 1000m)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == "Sneakers");
        if (category == null)
        {
            category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Sneakers",
                Description = "Test",
                Type = CategoryType.Footwear,
                Slug = "sneakers",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Categories.Add(category);
        }
        else
        {
            db.Attach(category);
        }
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Sneaker",
            Description = "Integration test product",
            Price = price,
            Sku = "TEST-" + Guid.NewGuid().ToString("N")[..8],
            StockQuantity = 0,
            TrackQuantity = false,
            Status = ProductStatus.Active,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product.Id;
    }
}
