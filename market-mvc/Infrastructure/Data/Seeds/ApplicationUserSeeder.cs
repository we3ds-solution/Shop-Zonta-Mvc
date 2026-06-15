using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using market_mvc.Domain.entity;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// Seeder for creating initial application users (admin, demo accounts)
    /// Must run BEFORE other seeders that reference users
    /// </summary>
    public class ApplicationUserSeeder : ISeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string Name => "Application Users and Roles";
        public int Order => 0; // Must run first

        public ApplicationUserSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync(DbContext context)
        {
            var appContext = (AppDbContext)context;

            // Create roles first
            await CreateRolesAsync();

            // Create admin user
            await CreateAdminUserAsync();

            // Create demo users
            await CreateDemoUsersAsync();

            await appContext.SaveChangesAsync();
        }

        private async Task CreateRolesAsync()
        {
            var roles = new[] { "Admin", "Manager", "Support", "Customer" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task CreateAdminUserAsync()
        {
            const string adminEmail = "admin@market-mvc.local";
            const string adminUsername = "admin";
            const string adminPassword = "REMOVED-PASSWORD";

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin != null)
                return;

            var adminUser = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}"
                );
            }

            // Assign admin role
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }

        private async Task CreateDemoUsersAsync()
        {
            var demoUsers = new[]
            {
                new { Email = "manager@market-mvc.local", Username = "manager", Name = "Store Manager", Role = "Manager", Password = "REMOVED-PASSWORD" },
                new { Email = "support@market-mvc.local", Username = "support", Name = "Customer Support", Role = "Support", Password = "REMOVED-PASSWORD" },
                new { Email = "customer@market-mvc.local", Username = "customer", Name = "Demo Customer", Role = "Customer", Password = "REMOVED-PASSWORD" }
            };

            foreach (var demoUser in demoUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(demoUser.Email);
                if (existingUser != null)
                    continue;

                var user = new ApplicationUser
                {
                    UserName = demoUser.Username,
                    Email = demoUser.Email,
                    EmailConfirmed = true,
                    FullName = demoUser.Name,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user, demoUser.Password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create demo user {demoUser.Username}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}"
                    );
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, demoUser.Role);
            }
        }
    }
}
