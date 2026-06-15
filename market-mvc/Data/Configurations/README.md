# Data Configurations

This directory contains Entity Framework Core entity type configurations using the fluent API.

## Overview

Each configuration file defines the database schema, relationships, indexes, and constraints for a specific entity. This approach keeps the `AppDbContext` clean and maintainable.

## Structure

### Entity Configurations

- **ProductConfiguration.cs** - Product entity mapping and indexes
- **OrderConfiguration.cs** - Order entity mapping, value objects, and relationships
- **OrderItemConfiguration.cs** - Order item mapping and relationships
- **CategoryConfiguration.cs** - Category entity mapping
- **CustomerConfiguration.cs** - Customer entity mapping with value objects
- **ItemConfiguration.cs** - Item entity mapping

### Global Configuration

- **GlobalEntityConfiguration.cs** - Global configurations applied to all entities (soft delete filter)

## Key Features

### Soft Delete Filter

Automatically applied to all entities inheriting from `BaseEntity`:

```csharp
// Automatically filters out deleted records
context.Products.ToList(); // Only returns non-deleted products
```

### Value Objects

Properly configured owned entities for complex value types:

```csharp
// Order entity
entity.OwnsOne(o => o.CustomerName, name => { /* ... */ });
entity.OwnsOne(o => o.ShippingAddress, address => { /* ... */ });

// Customer entity
entity.OwnsOne(c => c.Name, name => { /* ... */ });
entity.OwnsOne(c => c.Address, address => { /* ... */ });
```

### Relationships

Clear definition of relationships with appropriate delete behaviors:

```csharp
// Soft delete cascading
entity.HasOne(o => o.Customer)
    .WithMany(c => c.Orders)
    .OnDelete(DeleteBehavior.SetNull); // Set to null on customer delete

// Hard delete cascading
entity.HasOne(oi => oi.Order)
    .WithMany(o => o.OrderItems)
    .OnDelete(DeleteBehavior.Cascade); // Delete order items with order
```

### Indexes

Strategic indexes for query performance:

```csharp
// Unique indexes with soft delete filter
entity.HasIndex(p => p.Sku)
    .IsUnique()
    .HasFilter("[Sku] IS NOT NULL AND [IsDeleted] = 0");

// Composite indexes
entity.HasIndex(oi => new { oi.OrderId, oi.ProductId });

// Regular performance indexes
entity.HasIndex(p => p.CreatedAt);
entity.HasIndex(p => p.Status);
```

## Configuration Pattern

Each configuration follows this pattern:

```csharp
public class YourEntityConfiguration : IEntityTypeConfiguration<YourEntity>
{
    public void Configure(EntityTypeBuilder<YourEntity> entity)
    {
        // 1. Configure properties (precision, length, required)
        entity.Property(e => e.Property)
            .HasMaxLength(100)
            .IsRequired();

        // 2. Configure value objects
        entity.OwnsOne(e => e.ValueObject, vo => { /* ... */ });

        // 3. Configure relationships
        entity.HasOne(e => e.Related)
            .WithMany(r => r.Items)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Configure indexes
        entity.HasIndex(e => e.Property);
    }
}
```

## Adding a New Entity Configuration

### Step 1: Create Configuration File

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market_mvc.Models.entity;

namespace market_mvc.Data.Configurations
{
    public class YourEntityConfiguration : IEntityTypeConfiguration<YourEntity>
    {
        public void Configure(EntityTypeBuilder<YourEntity> entity)
        {
            // Your configuration here
        }
    }
}
```

### Step 2: Register in AppDbContext

In `AppDbContext.OnModelCreating()`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Apply global configurations
    modelBuilder.ApplyGlobalConfigurations();
    
    // Apply specific configurations
    modelBuilder.ApplyConfiguration(new ProductConfiguration());
    modelBuilder.ApplyConfiguration(new OrderConfiguration());
    modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    modelBuilder.ApplyConfiguration(new ItemConfiguration());
    
    // Add your new configuration here
    modelBuilder.ApplyConfiguration(new YourEntityConfiguration());
}
```

### Step 3: Update AppDbContext DbSet

Add the DbSet property if not already present:

```csharp
public DbSet<YourEntity> YourEntities { get; set; }
```

## Precision and Types

### Decimal Precision

All monetary values use `decimal(18, 2)`:

```csharp
entity.Property(p => p.Price).HasPrecision(18, 2);
// Supports values up to 9,999,999,999,999,999.99
```

### Enum Conversion

Enums are stored as integers for better performance:

```csharp
entity.Property(p => p.Status).HasConversion<int>();
```

### String Length

- **Names**: 100-500 characters
- **Emails**: 255 characters
- **URLs**: 500 characters
- **Descriptions**: 1000-2000 characters

## Index Naming Convention

Indexes follow this naming pattern:

- Unique indexes: `IX_{Entity}_{Property}_Unique`
- Composite indexes: `IX_{Entity}_{Property1}_{Property2}`
- Regular indexes: `IX_{Entity}_{Property}`
- Foreign keys: `FK_{Entity}_{RelatedEntity}`

## Migration and Deployment

When adding or modifying configurations:

```bash
# Create migration
dotnet ef migrations add {DescriptiveYourChange}

# Apply to database
dotnet ef database update

# Verify migration
dotnet ef migrations list
```

## Troubleshooting

### Issue: "The property ... is of type ... which does not have a backing field"

**Solution**: Ensure property is mapped correctly with `.Property(e => e.PropertyName)`

### Issue: "Unable to determine the relationship represented by navigation..."

**Solution**: Explicitly configure both sides of the relationship using `.HasOne()` and `.WithMany()`

### Issue: "The instance of entity type ... cannot be tracked"

**Solution**: Ensure soft delete filters are applied: `modelBuilder.ApplyGlobalConfigurations()`

## Best Practices

- ✅ Keep one entity per configuration file
- ✅ Use descriptive index names
- ✅ Document complex value object mappings
- ✅ Apply global configurations first
- ✅ Use appropriate delete behaviors
- ✅ Index frequently queried properties

- ❌ Don't hardcode magic numbers
- ❌ Don't skip value object configuration
- ❌ Don't create unnecessary indexes
- ❌ Don't modify global configuration without testing

## Performance Considerations

1. **Indexes**: Created on frequently filtered/sorted columns
2. **Soft Delete**: Uses filtered indexes for better performance
3. **Precision**: Balanced between accuracy and storage
4. **Relationships**: Configured with appropriate cascade behaviors

## Related Files

- `AppDbContext.cs` - Main database context
- `../Seeds/` - Database seeding
- `../../Models/` - Entity definitions

## Documentation

For more information about EF Core configurations:
- [Entity Type Configuration](https://docs.microsoft.com/en-us/ef/core/modeling/entity-types)
- [Relationships](https://docs.microsoft.com/en-us/ef/core/modeling/relationships)
- [Indexes](https://docs.microsoft.com/en-us/ef/core/modeling/indexes)
