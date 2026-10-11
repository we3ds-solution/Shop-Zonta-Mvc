using market_mvc.Features.Catalog;
using market_mvc.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Register services
// --------------------
builder.Services.AddAllApplicationServices(builder.Configuration, builder.Environment);

// --------------------
// Maintainable manual catalog import (does not start the web server):
//   dotnet run --project market-mvc -- import-catalog --file <path-to-ZontaCatalog.json>
// Idempotent by Sku; never duplicates; preserves manual stock (see CatalogImportService).
// --------------------
if (args.Any(a => string.Equals(a, "import-catalog", StringComparison.OrdinalIgnoreCase)))
{
    var fileIndex = Array.FindIndex(args, a => string.Equals(a, "--file", StringComparison.OrdinalIgnoreCase));
    var file = fileIndex >= 0 && fileIndex + 1 < args.Length
        ? args[fileIndex + 1]
        : Path.Combine(builder.Environment.ContentRootPath, "Infrastructure", "Data", "Catalog", "ZontaCatalog.json");

    var importApp = builder.Build();
    using var scope = importApp.Services.CreateScope();
    var importer = scope.ServiceProvider.GetRequiredService<ICatalogImportService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var (ok, error, result) = await importer.ImportAsync(file);
    if (!ok)
    {
        logger.LogError("Catalog import failed: {Error}", error);
        Console.Error.WriteLine($"Catalog import failed: {error}");
        return 1;
    }
    logger.LogInformation("Catalog import: {Created} created, {Updated} updated, {Variants} variants. {Error}",
        result!.Created, result.Updated, result.VariantsCreated, error ?? "OK");
    Console.WriteLine($"Catalog import: {result.Created} created, {result.Updated} updated, {result.VariantsCreated} variants created." +
        (error != null ? $" Warnings: {error}" : ""));
    return 0;
}

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
app.UseSession();
app.UseRequestLocalization(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);
app.UseRoutingConfiguration(app.Environment);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
await app.SeedDatabaseAsync();

app.Run();
return 0;

public partial class Program
{
    // Exposed for WebApplicationFactory integration tests (issue #8).
}
