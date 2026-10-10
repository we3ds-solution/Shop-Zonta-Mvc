using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using market_mvc.Domain.entity;

namespace market_mvc.Infrastructure.Data.Seeds
{
    /// <summary>
    /// Seeds roles (always, idempotent) and optionally provisions the first admin
    /// from environment-provided bootstrap credentials. No hardcoded passwords.
    /// Demo accounts are created in Development only and never in production.
    /// </summary>
    public class ApplicationUserSeeder : ISeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ApplicationUserSeeder> _logger;

        public string Name => "Application Users and Roles";
        public int Order => 0;

        public ApplicationUserSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILogger<ApplicationUserSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _env = env;
            _logger = logger;
        }

        public async Task SeedAsync(DbContext context)
        {
            var appContext = (AppDbContext)context;

            await CreateRolesAsync();
            await ProvisionBootstrapAdminAsync();
            if (_env.IsDevelopment())
            {
                await CreateDevelopmentDemoUserAsync();
            }

            await appContext.SaveChangesAsync();
        }

        private async Task CreateRolesAsync()
        {
            var roles = new[] { "Admin", "Manager", "Support", "Customer" };
            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private async Task ProvisionBootstrapAdminAsync()
        {
            // Explicit opt-in only: ZONTA_SEED_ADMIN=true + email + strong password via env/secret manager.
            var enabled = string.Equals(
                Environment.GetEnvironmentVariable("ZONTA_SEED_ADMIN"),
                "true", StringComparison.OrdinalIgnoreCase)
                || _configuration.GetValue<bool>("AdminBootstrap:Enabled");

            if (!enabled)
            {
                _logger.LogInformation("Admin bootstrap disabled (set ZONTA_SEED_ADMIN=true to enable).");
                return;
            }

            var email = Environment.GetEnvironmentVariable("ZONTA_ADMIN_EMAIL")
                ?? _configuration["AdminBootstrap:Email"];
            var password = Environment.GetEnvironmentVariable("ZONTA_ADMIN_PASSWORD")
                ?? _configuration["AdminBootstrap:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Admin bootstrap enabled but ZONTA_ADMIN_EMAIL/PASSWORD not provided. Skipping.");
                return;
            }

            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                _logger.LogInformation("Bootstrap admin already exists. Skipping creation.");
                return;
            }

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Zonta Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create bootstrap admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to assign Admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }

            // Never log the password.
            _logger.LogInformation("Bootstrap admin provisioned for {Email}. Rotate/remove bootstrap credentials now.", email);
        }

        private async Task CreateDevelopmentDemoUserAsync()
        {
            const string email = "demo.customer@zonta.local";
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return;
            }

            // Development-only demo account with a random password (logged once as warning-free? do not log).
            // Developers can use Register page instead; this account exists only for local smoke tests.
            var password = "Demo-" + Guid.NewGuid().ToString("N")[..12] + "aA1!";
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Demo Customer (dev only)",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                _logger.LogInformation("Development demo customer created (password not logged; use Register page or reset via Identity).");
            }
        }
    }
}
