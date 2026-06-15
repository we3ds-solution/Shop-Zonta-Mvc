# Seeds Directory

This directory contains database seeders for populating the database with initial data.

## Overview

The seeding system uses an orchestrator pattern to manage multiple independent seeders that run in a specific order. Each seeder is responsible for seeding a specific entity type.

## Architecture

### Core Components

- **ISeeder** - Interface for all seeders
- **SeederOrchestrator** - Orchestrates execution of all seeders in priority order
- **CategorySeeder** - Seeds product categories
- **CustomerSeeder** - Seeds customer data
- **ProductSeeder** - Seeds product data
- **OrderSeeder** - Seeds order and order item data

### Execution Order

Seeders execute in priority order (defined by `Order` property):

1. **CategorySeeder** (Order: 1) - Must run first as products depend on categories
2. **CustomerSeeder** (Order: 2) - Must run early as orders depend on customers
3. **ProductSeeder** (Order: 3) - Requires categories to be seeded
4. **OrderSeeder** (Order: 4) - Requires customers and products to be seeded

## How to Add a New Seeder

### Step 1: Create a New Seeder Class

```csharp
using Microsoft.EntityFrameworkCore;
using market_mvc.Data;

namespace market_mvc.Seeds
{
    public class YourEntitySeeder : ISeeder
    {
        public string Name => "Your Entity";
        public int Order => 5; // Set appropriate order
        
        public async Task SeedAsync(DbContext context)
        {
            var appDbContext = context as AppDbContext;
            if (appDbContext == null)
                throw new InvalidOperationException("Context must be AppDbContext");
            
            // Check if data already exists
            if (await appDbContext.YourEntities.AnyAsync())
                return;
            
            // Create and add data
            var entities = new List<YourEntity> { /* ... */ };
            await appDbContext.YourEntities.AddRangeAsync(entities);
            await appDbContext.SaveChangesAsync();
        }
    }
}
```

### Step 2: Register in Program.cs

Add the seeder to the service collection in `Program.cs`:

```csharp
builder.Services.AddSingleton<ISeeder, YourEntitySeeder>();
```

### Step 3: Set Correct Order Priority

Ensure the `Order` property reflects dependencies:
- Lower numbers execute first
- If your seeder depends on another, set a higher order value

## Best Practices

### ✅ DO

- **Check for existing data** before seeding to allow idempotent execution
- **Use descriptive seeder names** that reflect the entity being seeded
- **Add audit fields** (CreatedAt, CreatedBy) to seeded entities
- **Keep seeders focused** - one seeder per entity type
- **Add XML documentation** to explain the seeder's purpose
- **Handle dependencies properly** - ensure dependent entities are seeded first
- **Test seeding locally** before committing changes

### ❌ DON'T

- **Hardcode database values** that might change - use configuration if needed
- **Seed duplicate data** - check if data exists first
- **Create overly complex seeders** - keep them simple and maintainable
- **Skip order dependencies** - respect the execution order
- **Mix unrelated entities** in one seeder
- **Add production data** to seeders - keep it for development only

## Usage Examples

### Example 1: Simple Entity Seeder

```csharp
public class TagSeeder : ISeeder
{
    public string Name => "Tags";
    public int Order => 2; // Early execution

    public async Task SeedAsync(DbContext context)
    {
        var appDbContext = context as AppDbContext;
        if (appDbContext == null)
            throw new InvalidOperationException("Context must be AppDbContext");

        if (await appDbContext.Tags.AnyAsync())
            return;

        var tags = new[]
        {
            new Tag { Id = Guid.NewGuid(), Name = "New", IsActive = true },
            new Tag { Id = Guid.NewGuid(), Name = "Sale", IsActive = true },
            new Tag { Id = Guid.NewGuid(), Name = "Featured", IsActive = true }
        };

        await appDbContext.Tags.AddRangeAsync(tags);
        await appDbContext.SaveChangesAsync();
    }
}
```

### Example 2: Seeder with Dependencies

```csharp
public class BlogPostSeeder : ISeeder
{
    public string Name => "Blog Posts";
    public int Order => 5; // After categories

    public async Task SeedAsync(DbContext context)
    {
        var appDbContext = context as AppDbContext;
        if (appDbContext == null)
            throw new InvalidOperationException("Context must be AppDbContext");

        if (await appDbContext.BlogPosts.AnyAsync())
            return;

        var categories = await appDbContext.Categories.ToListAsync();
        var categoryId = categories.FirstOrDefault()?.Id;

        if (!categoryId.HasValue)
            return; // Categories must be seeded first

        var posts = new[]
        {
            new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = "My First Post",
                Content = "Content here...",
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Seeder"
            }
        };

        await appDbContext.BlogPosts.AddRangeAsync(posts);
        await appDbContext.SaveChangesAsync();
    }
}
```

## Seeder Execution Flow

```
Program.cs
    ↓
SeederOrchestrator.ExecuteAsync()
    ↓
Database.EnsureCreatedAsync()
    ↓
Sort seeders by Order property
    ↓
For each seeder (in order):
    ├─ Log start
    ├─ Execute SeedAsync()
    ├─ Log completion
    └─ Handle errors
    ↓
Log completion or error
```

## Troubleshooting

### Issue: Seeder runs before dependency seeder

**Solution:** Check the `Order` property. Dependencies should have a lower order value.

### Issue: Data is duplicated after seeding

**Solution:** Ensure the seeder checks `if (await context.Entity.AnyAsync()) return;` before seeding.

### Issue: Foreign key constraint error

**Solution:** Verify that dependent seeders have lower `Order` values and are executed first.

### Issue: Seeder not executing

**Solution:** Verify that the seeder is registered in `Program.cs` with `builder.Services.AddSingleton<ISeeder, YourSeeder>();`

## Testing Seeders

### Reset Database for Testing

```bash
dotnet ef database drop --force
dotnet ef database update
```

This will:
1. Drop the existing database
2. Create a new database with migrations
3. Execute all seeders in order

### Verify Seeded Data

```csharp
// In Program.cs or a test
var seeders = orchestrator.GetRegisteredSeeders();
foreach (var seeder in seeders)
{
    Console.WriteLine($"[{seeder.Order}] {seeder.Name}");
}
```

## Future Enhancements

- [ ] Configuration-based seeding
- [ ] CSV/JSON import support
- [ ] Selective seeding (seed only specific entities)
- [ ] Seeder versioning
- [ ] Performance metrics logging
- [ ] Rollback capability

## Related Files

- `Program.cs` - Seeder registration and execution
- `Data/AppDbContext.cs` - Database context
- `Models/` - Entity definitions

## License

Same as the main project (MIT)
