using market_mvc.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Register services
// --------------------
builder.Services.AddAllApplicationServices(builder.Configuration);

// --------------------
// Build app
// --------------------
var app = builder.Build();

// --------------------
// Middleware pipeline
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseProductionPipeline();
}
else
{
    app.UseDevelopmentPipeline();
}

app.UseCustomMiddleware();
app.UseStatusCodeConfiguration();
app.UseSecurityConfiguration();
app.UseStaticFiles();
app.UseRoutingConfiguration(app.Environment);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
await app.SeedDatabaseAsync();

app.Run();

