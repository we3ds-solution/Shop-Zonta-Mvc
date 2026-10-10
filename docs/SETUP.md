# Setup & Development Guide

## Development Environment Setup

### System Requirements

- **OS**: Windows 10/11, macOS 12+, or Linux
- **.NET SDK**: 9.0 or later
- **SQL Server**: 2019 or later (or SQL Server Express)
- **IDE**: Visual Studio 2022, VS Code, or Rider
- **RAM**: 4GB minimum (8GB recommended)
- **Disk Space**: 2GB minimum

### Installation Steps

#### 1. Install .NET SDK
Download from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

Verify installation:
```bash
dotnet --version
```

#### 2. Install SQL Server
- **Option A**: [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-express)
- **Option B**: [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-2022)
- **Option C**: Docker
  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server
  ```

#### 3. Install Visual Studio or VS Code
- [Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/)
- [VS Code](https://code.visualstudio.com/) + C# Dev Kit extension

#### 4. Clone Repository
```bash
git clone https://github.com/Mostafa-SAID7/Marketing-Mvc.git
cd Marketing-Mvc
```

#### 5. Restore Dependencies
```bash
dotnet restore
```

#### 6. Configure Database Connection

**Edit `market-mvc/appsettings.Development.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=market_mvc_db;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

**Windows Authentication (Windows only):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=market_mvc_db;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### 7. Create Database & Run Migrations

```bash
cd market-mvc
dotnet ef database update
```

This will:
- Create the `market_mvc_db` database
- Apply all migrations
- Seed initial data (if seeder is configured)

#### 8. Run the Application

```bash
dotnet run
```

Or using Visual Studio:
- Open `market-mvc.sln`
- Press `F5` or `Ctrl+F5`

**Application URL**: `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP)

## Development Workflow

### Creating a New Feature

1. **Create Feature Folder**
   ```
   Features/YourFeature/
   ├── Commands/
   ├── Queries/
   ├── Handlers/
   ├── Validators/
   └── Dtos/
   ```

2. **Define DTO**
   ```csharp
   public class CreateYourFeatureCommand : IRequest<YourFeatureResponse>
   {
       public string Name { get; set; }
   }
   ```

3. **Create Validator**
   ```csharp
   public class CreateYourFeatureValidator : BaseValidator<CreateYourFeatureCommand>
   {
       public CreateYourFeatureValidator()
       {
           RuleFor(x => x.Name).NotEmpty();
       }
   }
   ```

4. **Create Handler**
   ```csharp
   public class CreateYourFeatureHandler : IRequestHandler<CreateYourFeatureCommand, YourFeatureResponse>
   {
       public async Task<YourFeatureResponse> Handle(CreateYourFeatureCommand request, CancellationToken cancellationToken)
       {
           // Implementation
       }
   }
   ```

5. **Register Services** in `Program.cs`
   ```csharp
   builder.Services.AddScoped<IYourService, YourService>();
   ```

6. **Create Controller** in `Controllers/`
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class YourFeatureController : ControllerBase
   {
       private readonly IMediator _mediator;
       
       public YourFeatureController(IMediator mediator)
       {
           _mediator = mediator;
       }
       
       [HttpPost]
       public async Task<ActionResult> Create([FromBody] CreateYourFeatureCommand command)
       {
           var result = await _mediator.Send(command);
           return Ok(result);
       }
   }
   ```

### Database Migrations

**Create New Migration**
```bash
dotnet ef migrations add YourMigrationName
```

**Update Database**
```bash
dotnet ef database update
```

**Revert Last Migration**
```bash
dotnet ef migrations remove
```

**Reset Database (Development Only)**
```bash
dotnet ef database drop --force
dotnet ef database update
```

## Debugging

### Visual Studio
- Press `F5` to start debugging
- Set breakpoints by clicking line numbers
- Use Debug > Windows menu for debugging windows

### VS Code
- Install C# Dev Kit extension
- Open `.vscode/launch.json` and select "net 5.0+" launch configuration
- Press `F5` to debug

### Browser Developer Tools
- Press `F12` or right-click → Inspect
- Console tab for JavaScript errors
- Network tab to inspect HTTP requests

## Code Quality

### Run Linting & Analysis
```bash
dotnet build /p:EnforceCodeStyleInBuild=true
```

### Code Formatting
- Install Prettier extension for VS Code
- Use `Ctrl+Shift+P` → Format Document

## Common Issues & Solutions

### Issue: "Database connection failed"
**Solution**: 
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database user has correct permissions

### Issue: "Migrations not applying"
**Solution**:
```bash
dotnet ef database update --verbose
```

### Issue: "Port 5001 already in use"
**Solution**:
```bash
# Change port in launchSettings.json
# Or find and kill process using the port
```

### Issue: "Entity Framework not recognizing changes"
**Solution**:
```bash
dotnet ef migrations add YourMigration
dotnet ef database update
```

## Performance Testing

### Load Testing with Apache Bench
```bash
ab -n 1000 -c 10 https://localhost:5001/
```

### Profiling
- Use Visual Studio Profiler (Debug > Performance Profiler)
- Monitor memory usage and CPU

## Deployment Preparation

### Before Production

1. Set `ASPNETCORE_ENVIRONMENT=Production`
2. Update connection strings to production database
3. Configure authentication secrets
4. Enable HTTPS with valid certificate
5. Run security scan: `dotnet package add OwaспTop`
6. Test with production database backup

### Build for Release
```bash
dotnet publish -c Release -o ./publish
```

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [AutoMapper Documentation](https://docs.automapper.org)
- [FluentValidation Documentation](https://fluentvalidation.net)

---

For troubleshooting, check logs in `bin/Debug/net9.0/logs/`
