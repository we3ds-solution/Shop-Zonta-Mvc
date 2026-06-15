# Market MVC - Quick Start Guide

## Prerequisites

- .NET SDK 9.0+
- SQL Server (local or remote)
- Visual Studio 2024 / VS Code / Rider (optional)

## Connection String

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db56070.public.databaseasp.net;Database=db56070;User Id=db56070;Password=REMOVED-DB-PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

## Build and Run

### Step 1: Clear NuGet Cache
```bash
dotnet nuget locals all --clear
```

### Step 2: Restore Packages
```bash
dotnet restore --no-cache
```

### Step 3: Build Project
```bash
dotnet build
```

### Step 4: Apply Database Migrations
```bash
dotnet ef database update
```

This will:
- Create all database tables
- Seed initial data:
  - 4 system roles: Admin, Manager, Support, Customer
  - Admin user: admin@market-mvc.local (REMOVED-PASSWORD)
  - Demo users for each role
  - Sample products and categories
  - Sample orders

### Step 5: Run Application
```bash
dotnet run
```

App runs on: `https://localhost:5001`

## Default Accounts

After database initialization, use these credentials:

### Admin Account
- Email: `admin@market-mvc.local`
- Password: `REMOVED-PASSWORD`
- Access: Full system access, product management, user management

### Manager Account
- Email: `manager@market-mvc.local`
- Password: `REMOVED-PASSWORD`
- Access: Product and order management

### Support Account
- Email: `support@market-mvc.local`
- Password: `REMOVED-PASSWORD`
- Access: View-only for orders

### Customer Account
- Email: `customer@market-mvc.local`
- Password: `REMOVED-PASSWORD`
- Access: Browse products, view own orders

## Testing

### Test Login Flow
1. Navigate to `/Auth/Login`
2. Enter admin credentials
3. Should redirect to home page

### Test Protected Routes
1. Logout (no access to admin features)
2. Navigate to `/Products/Create`
3. Should redirect to `/Auth/Login` or `/Auth/AccessDenied`
4. Login as customer
5. Try to create product → Access Denied
6. Logout and login as manager
7. Can create products

### Test Registration
1. Go to `/Auth/Register`
2. Fill form with:
   - Full Name: Test User
   - Email: testuser@example.com
   - Password: TestPass123!
   - Confirm password
   - Agree to terms
3. Should create account and auto-login
4. Auto-assigned Customer role

## Project Structure

```
market-mvc/
├── Controllers/              # MVC Controllers
│   ├── AuthController.cs     # Login/Register/Logout
│   ├── ProductsController.cs # Product CRUD (protected)
│   ├── OrdersController.cs   # Order management (protected)
│   ├── HomeController.cs
│   └── ErrorController.cs
├── Views/                    # Razor views
│   ├── Auth/                 # Login, Register, AccessDenied
│   ├── Products/
│   ├── Orders/
│   └── Home/
├── Domain/                   # Business entities & value objects
│   ├── entity/               # EF Core entities
│   │   ├── ApplicationUser.cs    # Identity user
│   │   ├── Customer.cs
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   └── ...
│   ├── ViewModels/           # Data transfer objects
│   │   ├── Auth/
│   │   ├── Product/
│   │   └── ...
│   ├── Enums/
│   ├── ObjectValues/         # Value objects
│   └── Interfaces/           # Repository interfaces
├── Infrastructure/           # Data access & services
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/   # EF configurations
│   │   ├── Seeds/            # Database seeders
│   │   └── Migrations/       # EF migrations
│   ├── Repositories/
│   ├── Services/
│   ├── Middleware/
│   └── Extensions/           # DI extensions
├── Features/                 # Feature modules (CQRS)
│   ├── Common/
│   ├── Home/
│   ├── Products/
│   ├── Orders/
│   └── Auth/
├── Program.cs               # Entry point
├── appsettings.json         # Configuration
└── market-mvc.csproj        # Project file
```

## Key Features

### Authentication
- User registration with email
- Login with remember-me
- Password complexity requirements
- Account lockout (5 attempts, 15 min timeout)
- Logout functionality

### Authorization
- Role-based access control (4 roles)
- Policy-based authorization
- Protected controllers and actions
- Access denied page

### Data Management
- Product catalog with CRUD operations
- Order management
- Customer profiles
- Soft delete support
- Audit fields (CreatedAt, UpdatedAt, etc.)

### Architecture
- Clean/Onion architecture
- CQRS pattern with MediatR
- Repository pattern
- Dependency injection
- FluentValidation
- AutoMapper

## Troubleshooting

### Build Issues

**NuGet Error: "The local source 'C:\Program Files...' doesn't exist"**
```bash
# Clear cache
dotnet nuget locals all --clear

# Set environment variable
set NUGET_PACKAGES=C:\Users\%USERNAME%\.nuget\packages

# Try restore again
dotnet restore --no-cache
```

### Database Issues

**Migration not found**
```bash
# Create new migration
dotnet ef migrations add "DescriptionOfChange"

# Apply it
dotnet ef database update
```

**Database locked**
- Close other connections to database
- Restart SQL Server service
- Check connection string

### Runtime Issues

**"InvalidOperationException: Unable to resolve service"**
- Check service is registered in Program.cs
- Ensure dependencies are scoped correctly
- Check ServiceRegistrationExtensions.cs

**"Access Denied" on protected page**
- User not logged in → redirect to login
- User has wrong role → insufficient permissions
- Account is inactive (IsActive = false)

## Development Tips

### Add New Feature
1. Create feature folder in `Features/FeatureName/`
2. Add CQRS: Queries/, Commands/, Handlers/
3. Create service in `Services/`
4. Add mappings in `Mappings/MappingProfile.cs`
5. Create controller actions
6. Add views in `Views/FeatureName/`

### Add New Entity
1. Create entity in `Domain/entity/`
2. Add DbSet in AppDbContext
3. Create configuration in `Infrastructure/Data/Configurations/`
4. Create repository and interface
5. Create seeder if needed
6. Create migration: `dotnet ef migrations add "AddNewEntity"`

### Modify User Authentication
- Edit password policy in `AuthenticationExtensions.cs`
- Edit lockout settings
- Edit cookie settings
- Edit roles in `ApplicationUserSeeder.cs`

## Documentation

- `IDENTITY_INTEGRATION.md` - Complete Identity setup guide
- `Features/Auth/README.md` - Auth module documentation
- `docs/API.md` - API documentation
- `docs/ARCHITECTURE.md` - Architecture overview
- `docs/SETUP.md` - Initial setup guide

## Useful Commands

```bash
# Build
dotnet build

# Run
dotnet run

# Run with verbose output
dotnet run --verbose

# Create migration
dotnet ef migrations add "MigrationName"

# Update database
dotnet ef database update

# View help
dotnet ef --help

# List migrations
dotnet ef migrations list

# Watch for changes and rebuild
dotnet watch run

# Run tests (if added)
dotnet test

# Clean build
dotnet clean
dotnet build
```

## Next Steps

1. ✅ Complete Identity integration (done)
2. ⏳ Fix NuGet package source issue
3. 🔄 Create database migrations
4. 🔄 Test authentication flow
5. 🔄 Add email confirmation
6. 🔄 Add password reset
7. 🔄 Add user profile management
8. 🔄 Add admin panel
9. 🔄 Add audit logging
10. 🔄 Deploy to production

## Support

For issues or questions:
- Check IDENTITY_INTEGRATION.md
- Review Features/Auth/README.md
- See troubleshooting section above
- Check git commits for recent changes
