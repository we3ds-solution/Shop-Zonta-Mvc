# Authentication & Authorization

This module handles user authentication and authorization for the Market MVC application using ASP.NET Core Identity.

## Architecture

### Entities

#### ApplicationUser
- Extends `IdentityUser` from ASP.NET Core Identity
- Handles authentication, passwords, and two-factor authentication
- One-to-One relationship with Customer (business entity)
- Properties:
  - `CustomerId`: Foreign key to Customer
  - `FullName`: Display name
  - `IsActive`: Account status
  - `CreatedAt`: Account creation timestamp
  - `LastPasswordChangedAt`: Password change tracking
  - `LastLoginAt`: Login tracking
  - `LockoutReason`: Manual lockout reason
  - `Roles`: Denormalized roles string

#### Customer
- Business entity for customer profile
- No longer contains authentication fields (moved to ApplicationUser)
- Maintains: `Name`, `Email`, `Phone`, `Address`, `IsActive`, `Notes`
- One-to-One relationship with ApplicationUser
- Used by Orders to store customer info at order time

### Roles

Four built-in roles with hierarchical permissions:

1. **Admin** - Full system access
   - Create, read, update, delete products
   - Manage users and roles
   - View all orders
   - System configuration

2. **Manager** - Business operations
   - Create, read, update, delete products
   - View and update orders
   - Manage inventory

3. **Support** - Customer support staff
   - Read products
   - View orders (customer support)
   - Cannot modify core data

4. **Customer** - End users
   - Browse products
   - View own orders
   - Update profile

### Authorization Policies

Defined in `AuthorizationExtensions.cs`:

- `AdminOnly` - Requires Admin role
- `AdminOrManager` - Requires Admin or Manager role (used for product management)
- `SupportAndAbove` - Requires Admin, Manager, or Support role
- `Authenticated` - Any authenticated user
- `Customer` - All authenticated users (default for orders)

### Protected Controllers

#### ProductsController
- **Public**: Index, Details (browse products)
- **AdminOrManager**: Create, Edit, Delete products
- Uses `[Authorize(Policy = "AdminOrManager")]` attribute

#### OrdersController
- **Public**: None
- **Authenticated**: Index, Details (view orders)
- **AdminOrManager**: Edit, Delete orders (modify order status, remove)
- Uses `[Authorize]` and `[Authorize(Policy = "AdminOrManager")]` attributes

#### AuthController
- **Public**: Login, Register, AccessDenied
- Handles user authentication flow

## Seeding

### ApplicationUserSeeder
Runs first (Order = 0) during database initialization.

**Creates:**
- 4 application roles: Admin, Manager, Support, Customer
- Default admin user: `admin@market-mvc.local` / `REMOVED-PASSWORD`
- Demo users for testing:
  - manager@market-mvc.local (Manager role)
  - support@market-mvc.local (Support role)
  - customer@market-mvc.local (Customer role)

**Password Requirements:**
- Minimum 8 characters
- Requires: uppercase, lowercase, digit, special character

## Services

### Configuration

In `ServiceRegistrationExtensions.cs`:
```csharp
.AddAllApplicationServices(configuration)
// Automatically includes:
// - AddIdentityServices()
// - AddCookieAuthentication()
// - AddAuthorizationPolicies()
```

### Cookie Authentication
- Login path: `/Auth/Login`
- Logout path: `/Auth/Logout`
- Access denied path: `/Auth/AccessDenied`
- Expiration: 7 days with sliding window
- HttpOnly and Secure flags enabled

### Middleware
- `UseAuthentication()` - Validate credentials
- `UseAuthorization()` - Check policies
- Configured in `MiddlewareExtensions.cs`

## Views

Located in `Views/Auth/`:

- **Login.cshtml** - User login form
- **Register.cshtml** - New account registration
- **AccessDenied.cshtml** - 403 error page

## ViewModels

### LoginVM
- Email (required, email format)
- Password (required, 6-100 chars)
- RememberMe (bool)

### RegisterVM
- FullName (required, max 256 chars)
- Email (required, unique)
- Password (8+ chars, complex requirements)
- ConfirmPassword (must match Password)
- AgreeToTerms (required checkbox)

## Usage Examples

### Login
```
POST /Auth/Login
{
  "email": "admin@market-mvc.local",
  "password": "REMOVED-PASSWORD",
  "rememberMe": true
}
```

### Register
```
POST /Auth/Register
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "SecurePass@123",
  "confirmPassword": "SecurePass@123",
  "agreeToTerms": true
}
```

### Authorize Action
```csharp
[Authorize(Policy = "AdminOrManager")]
public async Task<IActionResult> ManageProduct(Guid id)
{
    // Only Admin or Manager can access
}
```

### Check User Role
```csharp
if (User.IsInRole("Admin"))
{
    // Admin-only code
}

var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
```

## Database Schema

### Identity Tables (Created by EF Migration)
- AspNetUsers - ApplicationUser entities
- AspNetRoles - Identity roles
- AspNetUserRoles - User role assignments
- AspNetUserClaims - User claims
- AspNetUserLogins - External login mappings
- AspNetRoleClaims - Role claims
- AspNetUserTokens - Account tokens

### Business Tables
- Customers - Customer profiles (updated: added ApplicationUserId FK)
- Products, Orders, OrderItems (unchanged)

## Future Enhancements

1. **Email Confirmation** - Verify email before account activation
2. **Two-Factor Authentication** - SMS or authenticator app
3. **Password Reset** - Email-based password recovery
4. **OAuth2/OpenID Connect** - Google, GitHub, Microsoft login
5. **Audit Logging** - Track login attempts, role changes
6. **Session Management** - Track active sessions, force logout
7. **Claims-based Authorization** - Fine-grained permissions
8. **API Authentication** - JWT tokens for API endpoints

## Troubleshooting

### "The local source 'C:\Program Files (x86)\Microsoft Visual Studio\Shared\NuGetPackages' doesn't exist"
- This is a system-level NuGet configuration issue
- Solution: Run `dotnet nuget locals all --clear` and rebuild
- The `RestoreSources` in `.csproj` helps override system sources

### Authentication not working
- Ensure middleware order is correct: `UseAuthentication()` → `UseAuthorization()`
- Check cookie policy settings in `AuthenticationExtensions.cs`
- Verify `[Authorize]` attributes are applied to protected actions

### User locked out
- Default: 5 failed login attempts lock account for 15 minutes
- Configure in `AddIdentityServices()` options

## Related Files

- `Domain/entity/ApplicationUser.cs` - User entity
- `Domain/entity/Customer.cs` - Customer entity (updated)
- `Infrastructure/Data/Configurations/ApplicationUserConfiguration.cs` - EF configuration
- `Infrastructure/Data/Seeds/ApplicationUserSeeder.cs` - Seed admin/demo users
- `Infrastructure/Extensions/AuthenticationExtensions.cs` - Service registration
- `Infrastructure/Extensions/AuthorizationExtensions.cs` - Policy definitions
- `Controllers/AuthController.cs` - Auth endpoints
- `Controllers/ProductsController.cs` - Protected with AdminOrManager
- `Controllers/OrdersController.cs` - Protected with Authorize
