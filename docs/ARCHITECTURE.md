# Architecture Overview

## Clean Architecture Pattern

This project implements **Clean Architecture** to ensure separation of concerns and maintainability.

### Layers

#### 1. **Domain Layer** (Innermost)
- Core business entities
- Business logic rules
- No external dependencies

#### 2. **Application Layer**
- Use cases (Commands & Queries via MediatR)
- Business rules orchestration
- DTOs and View Models
- Validators (FluentValidation)

#### 3. **Infrastructure Layer**
- Database access (EF Core)
- Repository implementations
- External services
- Data persistence

#### 4. **Presentation Layer** (Outermost)
- MVC Controllers
- Views (Razor)
- User interface

## CQRS Pattern

Command Query Responsibility Segregation separates read and write operations:

```
Features/
├── Products/
│   ├── Commands/
│   │   ├── CreateProduct/
│   │   ├── UpdateProduct/
│   │   └── DeleteProduct/
│   ├── Queries/
│   │   ├── GetProducts/
│   │   └── GetProductById/
│   ├── Validators/
│   ├── Handlers/
│   └── Dtos/
```

## Dependency Injection

MediatR and AutoMapper are configured in `Program.cs`:

```csharp
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
```

## Data Access Pattern

### Repository Pattern
- `IUnitOfWork` - Coordinates multiple repositories
- `IOrderRepository` - Order data access
- `IProductRepo` - Product data access
- `IHomeRepository` - Home page data

### Unit of Work
Manages database transactions and multiple repositories:

```csharp
public interface IUnitOfWork
{
    IOrderRepository OrderRepository { get; }
    IProductRepo ProductRepository { get; }
    Task SaveAsync();
}
```

## Entity Framework Core

### DbContext
- `AppDbContext` - Main database context
- Configured in `Data/AppDbContext.cs`
- Uses SQL Server provider

### Migrations
Located in `Migrations/` directory:
- `InitialCQRSImplementation`
- `InitialMigrationWithCleanArchitecture`

## Service Layer

### Key Services
- `OrderService` - Order business logic
- `ProductServ` - Product business logic
- `HomeService` - Dashboard and home page logic
- `ImageService` - File upload and image handling

## Validation

FluentValidation validators for CQRS commands:

```csharp
public class CreateProductCommandValidator : BaseValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required");
    }
}
```

## Mapping

AutoMapper profiles for DTO to Entity conversions:

```csharp
CreateMap<CreateProductCommand, Product>();
CreateMap<Product, ProductCardVM>();
```

## Cross-Cutting Concerns

### Infrastructure.Common
- `BaseCommand` - Base class for MediatR commands
- `BaseValidator` - Base class for FluentValidation validators
- `BaseSpecification` - Specification pattern for queries

### Models.Base
- `BaseEntity` - Base for all domain entities
- `ISoftDeletable` - Soft delete support
- `IAuditable` - Audit trail support

## Error Handling

Custom error handling middleware:
- Global exception handler
- Status code pages
- Logging integration

## Configuration

### appsettings.json
- Database connection string
- Logging levels
- Application settings

### appsettings.Development.json
- Development-specific overrides
- Detailed logging

## Security

### Authentication
- ASP.NET Core Identity
- User registration and login
- Password hashing

### Authorization
- Role-based access control (RBAC)
- Policy-based authorization
- Controller action authorization

## Performance Considerations

- Specification pattern for efficient queries
- Pagination support
- Entity Framework query optimization
- Static file caching

---

For detailed implementation, see individual feature documentation in `docs/features/`
