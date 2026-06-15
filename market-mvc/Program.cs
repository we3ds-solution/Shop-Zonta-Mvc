using Microsoft.EntityFrameworkCore;
using market_mvc.Data;
using market_mvc.Repositoriers;
using market_mvc.Services;
using market_mvc.Seeds;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Register services
// --------------------
builder.Services.AddControllersWithViews();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Existing services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductServ, ProductServ>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();

// Seeder registration
builder.Services.AddSingleton<SeederOrchestrator>();
builder.Services.AddSingleton<ISeeder, CategorySeeder>();
builder.Services.AddSingleton<ISeeder, CustomerSeeder>();
builder.Services.AddSingleton<ISeeder, ProductSeeder>();
builder.Services.AddSingleton<ISeeder, OrderSeeder>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// --------------------
// Build app
// --------------------
var app = builder.Build();

// --------------------
// Middleware pipeline
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var orchestrator = scope.ServiceProvider.GetRequiredService<SeederOrchestrator>();
    
    // Register all seeders
    var seeders = scope.ServiceProvider.GetServices<ISeeder>();
    foreach (var seeder in seeders)
    {
        orchestrator.RegisterSeeder(seeder);
    }
    
    // Execute seeding
    try
    {
        logger.LogInformation("Starting database seeding process");
        await orchestrator.ExecuteAsync(context);
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during database seeding");
    }
}

app.Run();
