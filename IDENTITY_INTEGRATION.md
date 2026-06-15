# ASP.NET Core Identity Integration Guide

## Overview

This document describes the ASP.NET Core Identity implementation added to the Market MVC project. The integration adds secure user authentication and role-based authorization to the application.

## Architecture

### Key Entities

#### ApplicationUser
Extends `IdentityUser` and represents authenticated users with:
- Email and username (from IdentityUser)
- Password hash (managed by Identity)
- FullName, IsActive, CreatedAt, LastLoginAt
- One-to-One relationship with Customer (business entity)

#### Customer
Business entity representing a customer profile:
- Personal info: Name, Email, Phone, Address
- Business fields: IsActive, Notes, TotalOrders, TotalSpent
- Now linked to ApplicationUser via FK `ApplicationUserId`

### Roles Hierarchy

```
Admin (highest privilege)
  ├─ Full system access
  ├─ Manage users and roles
  └─ All administrative functions

Manager
  ├─ Product management (CRUD)
  ├─ Order management (view/update)
  └─ Inventory management

Support
  ├─ View orders (customer service)
  ├─ View products
  └─ Read-only access

Customer (regular users)
  ├─ Browse products
  ├─ View own orders
  └─ Update profile
```

## Implementation Checklist

### ✅ COMPLETED

- [x] **ApplicationUser Entity** - Created in `Domain/entity/ApplicationUser.cs`
  - Extends IdentityUser with business fields
  - Foreign key to Customer for one-to-one relationship
  - Tracking: CreatedAt, LastLoginAt, LastPasswordChangedAt

- [x] **Customer Entity Update** - Modified `Domain/entity/Customer.cs`
  - Removed auth fields (LastLoginAt, EmailVerifiedAt, PhoneVerifiedAt)
  - Added ApplicationUserId foreign key
  - Removed auth methods (VerifyEmail, VerifyPhone, UpdateLastLogin)

- [x] **AppDbContext Update** - Modified `Infrastructure/Data/AppDbContext.cs`
  - Changed inheritance from DbContext to IdentityDbContext<ApplicationUser>
  - Added ApplicationUser DbSet (auto-added by IdentityDbContext)
  - Added ApplicationUserConfiguration to OnModelCreating

- [x] **ApplicationUserConfiguration** - Created `Infrastructure/Data/Configurations/ApplicationUserConfiguration.cs`
  - Entity Framework configuration for ApplicationUser
  - Relationships, constraints, indexes
  - One-to-One relationship with Customer (SetNull on delete)

- [x] **ApplicationUserSeeder** - Created `Infrastructure/Data/Seeds/ApplicationUserSeeder.cs`
  - Creates initial roles: Admin, Manager, Support, Customer
  - Admin user: admin@market-mvc.local (password: REMOVED-PASSWORD)
  - Demo users for testing each role
  - Seeder Order = 0 (runs first)

- [x] **AuthenticationExtensions** - Created `Infrastructure/Extensions/AuthenticationExtensions.cs`
  - AddIdentityServices() - Register Identity with custom password policy
  - AddCookieAuthentication() - Configure cookie settings
  - AddAuthenticationServices() - Combined registration

- [x] **AuthorizationExtensions** - Created `Infrastructure/Extensions/AuthorizationExtensions.cs`
  - Define authorization policies: AdminOnly, AdminOrManager, SupportAndAbove, Authenticated, Customer
  - Policies can be applied to controllers/actions

- [x] **AuthController** - Created `Controllers/AuthController.cs`
  - Login action: Authenticate user, update LastLoginAt
  - Register action: Create new user, assign Customer role
  - Logout action: Sign out and redirect
  - AccessDenied action: Display 403 page
  - Account lockout handling (5 attempts → 15 min lockout)

- [x] **Auth ViewModels** - Created in `Domain/ViewModels/Auth/`
  - LoginVM: Email, Password, RememberMe
  - RegisterVM: FullName, Email, Password, ConfirmPassword, AgreeToTerms

- [x] **Auth Views** - Created in `Views/Auth/`
  - Login.cshtml - User login form
  - Register.cshtml - Account registration form
  - AccessDenied.cshtml - 403 error page

- [x] **Protected Controllers** - Added [Authorize] attributes
  - ProductsController: Create/Edit/Delete require AdminOrManager
  - OrdersController: All actions require Authenticate, Edit/Delete require AdminOrManager
  - AuthController: Public endpoints (Login, Register)

- [x] **Service Registration** - Updated `ServiceRegistrationExtensions.cs`
  - AddAllApplicationServices() now calls AddAuthenticationServices()
  - AddAllApplicationServices() now calls AddAuthorizationPolicies()

- [x] **Seeder Registration** - Updated `ServiceRegistrationExtensions.cs`
  - ApplicationUserSeeder registered as Transient (Order = 0)
  - Runs before other seeders during database initialization

- [x] **Program.cs** - Already configured correctly
  - UseAuthentication() middleware is already in pipeline
  - UseAuthorization() middleware is already in pipeline
  - Database seeding already calls SeedDatabaseAsync()

### ⏳ BLOCKED BY NUGET ISSUE

The following steps require the project to build successfully. The NuGet package restore error needs to be resolved first:

**Error**: "The local source 'C:\Program Files (x86)\Microsoft Visual Studio\Shared\NuGetPackages' doesn't exist"

**Resolution Options**:

1. **System-level fix**:
   - Reinstall .NET SDK
   - Or remove Visual Studio and reinstall
   - Or configure NuGet sources in registry

2. **Quick workaround**:
   - In the project folder, run: `dotnet nuget locals all --clear`
   - Then try: `dotnet restore --no-cache`
   - Set environment: `set NUGET_PACKAGES=C:\Users\Memo\.nuget\packages`

3. **Already attempted**:
   - ✓ Added RestoreSources to .csproj
   - ✓ Added disabledPackageSources to NuGet.config
   - ✓ Cleared NuGet cache

### 🔄 NEXT STEPS (After Build Works)

1. **Create Database Migration**
   ```bash
   dotnet ef migrations add "AddIdentityIntegration" --project .
   ```
   This will:
   - Create tables for Identity (AspNetUsers, AspNetRoles, etc.)
   - Update Customers table (add ApplicationUserId column)

2. **Apply Migration**
   ```bash
   dotnet ef database update
   ```
   This will:
   - Create all Identity tables
   - Add foreign key constraints
   - Seed initial data (admin + demo users + products/categories)

3. **Test Authentication**
   - Run app: `dotnet run`
   - Navigate to `/Auth/Login`
   - Login as admin@market-mvc.local / REMOVED-PASSWORD
   - Try managing products (should work)
   - Logout and login as customer@market-mvc.local / REMOVED-PASSWORD
   - Try managing products (should show AccessDenied)

4. **Test Registration**
   - Go to `/Auth/Register`
   - Create new account
   - Auto-assigned Customer role
   - Can view orders but cannot manage them

5. **Optional Enhancements**
   - Email confirmation flow
   - Password reset functionality
   - Two-factor authentication
   - OAuth/OpenID Connect integration
   - Claims-based authorization
   - Audit logging

## Configuration Details

### Password Policy
```csharp
// From AuthenticationExtensions.cs
options.Password.RequireDigit = true;
options.Password.RequiredLength = 8;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequireUppercase = true;
options.Password.RequireLowercase = true;
```

### Lockout Policy
```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;
```

### Cookie Settings
```csharp
options.ExpireTimeSpan = TimeSpan.FromDays(7);
options.SlidingExpiration = true;
options.Cookie.HttpOnly = true;  // Prevents JavaScript access
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // HTTPS only
```

### Paths
- Login: `/Auth/Login`
- Logout: `/Auth/Logout`
- AccessDenied: `/Auth/AccessDenied`

## Database Schema Changes

### New Tables (Created by IdentityDbContext)
- **AspNetUsers** - Application users
- **AspNetRoles** - System roles
- **AspNetUserRoles** - User role mappings
- **AspNetUserClaims** - User claims
- **AspNetRoleClaims** - Role claims
- **AspNetUserTokens** - Account recovery tokens
- **AspNetUserLogins** - External login mappings

### Modified Tables
- **Customers** - Added ApplicationUserId (nullable FK to AspNetUsers)

### Unchanged Tables
- Products, Categories, Orders, OrderItems (no changes)

## Authorization Examples

### In Controllers
```csharp
// Require any authenticated user
[Authorize]
public async Task<IActionResult> MyOrders() { }

// Require specific role
[Authorize(Policy = "AdminOrManager")]
public async Task<IActionResult> CreateProduct() { }

// Require admin only
[Authorize(Policy = "AdminOnly")]
public async Task<IActionResult> ManageUsers() { }
```

### In Views
```html
@using Microsoft.AspNetCore.Identity

<!-- If user is authenticated -->
@if (User?.Identity?.IsAuthenticated == true)
{
    <p>Hello @User?.Identity?.Name</p>
    
    <!-- If user is admin -->
    @if (User.IsInRole("Admin"))
    {
        <a href="/admin/dashboard">Admin Panel</a>
    }
}
else
{
    <a href="/Auth/Login">Login</a>
    <a href="/Auth/Register">Register</a>
}
```

### In Service Code
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
var isAdmin = User.IsInRole("Admin");
```

## Troubleshooting

### Issue: "Cannot find type 'ApplicationUser'"
**Cause**: Using old name or wrong namespace
**Fix**: Use `market_mvc.Domain.entity.ApplicationUser`

### Issue: "Password does not meet complexity requirements"
**Cause**: Password doesn't meet policy (uppercase, lowercase, digit, special)
**Fix**: Use format like: `REMOVED-PASSWORD` or `Password123!`

### Issue: "User already exists"
**Cause**: Trying to create user with existing email
**Fix**: Use unique email or delete existing user first

### Issue: Access Denied when accessing protected action
**Possible causes**:
1. Not logged in - redirect to Login page
2. Wrong role - user has Customer role but endpoint requires Manager
3. Account locked - wait 15 minutes or reset
4. Email not verified (if configured)

**Fix**: 
- Check User.Identity.IsAuthenticated
- Check User.IsInRole("RoleName")
- Check user's account status (IsActive)

## File Structure

```
market-mvc/
├── Controllers/
│   ├── AuthController.cs (new)
│   ├── ProductsController.cs (updated - added [Authorize])
│   └── OrdersController.cs (updated - added [Authorize])
├── Domain/
│   ├── entity/
│   │   ├── ApplicationUser.cs (new)
│   │   └── Customer.cs (updated - removed auth fields)
│   └── ViewModels/
│       └── Auth/ (new)
│           ├── LoginVM.cs
│           └── RegisterVM.cs
├── Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs (updated - IdentityDbContext)
│   │   ├── Configurations/
│   │   │   └── ApplicationUserConfiguration.cs (new)
│   │   └── Seeds/
│   │       └── ApplicationUserSeeder.cs (new)
│   └── Extensions/
│       ├── AuthenticationExtensions.cs (new)
│       ├── AuthorizationExtensions.cs (new)
│       └── ServiceRegistrationExtensions.cs (updated)
├── Views/
│   └── Auth/ (new)
│       ├── Login.cshtml
│       ├── Register.cshtml
│       └── AccessDenied.cshtml
└── Features/
    └── Auth/
        └── README.md (new - detailed documentation)
```

## Related Documentation

- `Features/Auth/README.md` - Detailed auth module documentation
- `.csproj` - Already has Microsoft.AspNetCore.Identity.EntityFrameworkCore v9.0.10
- `appsettings.json` - Contains connection string

## Next Session Tasks

1. **Resolve NuGet issue** - Get dotnet build working
2. **Run migrations** - Create Identity tables
3. **Test authentication** - Login/register flow
4. **Add email confirmation** - Email verification
5. **Add password reset** - Forgot password flow
6. **Profile management** - User settings page
7. **Admin panel** - User management interface
