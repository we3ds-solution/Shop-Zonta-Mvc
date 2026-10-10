# Contributing to Market MVC

Thank you for your interest in contributing! This guide will help you get started.

## Code of Conduct

We are committed to providing a welcoming and inclusive environment. Please be respectful to all contributors.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally
3. **Create a feature branch** from `develop`
4. **Make your changes** following the guidelines below
5. **Push to your fork** and submit a pull request

## Development Workflow

### Branch Naming

Use descriptive branch names with prefixes:

- `feature/` - New features
- `fix/` - Bug fixes
- `refactor/` - Code refactoring
- `docs/` - Documentation changes
- `test/` - Test additions or fixes

**Examples:**
- `feature/product-filtering`
- `fix/order-status-update`
- `refactor/service-layer`
- `docs/api-endpoints`

### Commit Messages

Follow conventional commits format:

```
type(scope): subject

body

footer
```

**Types:**
- `feat` - A new feature
- `fix` - A bug fix
- `docs` - Documentation changes
- `style` - Code style changes (formatting, etc.)
- `refactor` - Code refactoring
- `perf` - Performance improvements
- `test` - Test additions or modifications
- `chore` - Build, dependencies, or tooling changes

**Examples:**
```
feat(products): add product filtering by category

- Implement category filter in ProductQuery
- Update ProductController with filter endpoint
- Add unit tests for filtering logic

Closes #123
```

```
fix(orders): resolve payment status not updating

Payment status was not persisting due to missing DbContext.SaveAsync() call.

Fixes #456
```

## Code Quality Standards

### C# Code Style

Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions):

- Use PascalCase for class names, methods, and properties
- Use camelCase for local variables and parameters
- Use underscore prefix for private fields: `_fieldName`
- Use meaningful names for variables and methods
- Keep lines under 120 characters

### Example:

```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Product> GetProductAsync(Guid id)
    {
        _logger.LogInformation("Retrieving product with ID: {ProductId}", id);
        return await _productRepository.GetByIdAsync(id);
    }
}
```

### Best Practices

1. **SOLID Principles**
   - Single Responsibility: Each class has one reason to change
   - Open/Closed: Open for extension, closed for modification
   - Liskov Substitution: Derived classes should be substitutable
   - Interface Segregation: Clients depend on specific interfaces
   - Dependency Inversion: Depend on abstractions, not concretions

2. **Clean Code**
   - Use descriptive names
   - Keep methods short and focused
   - Avoid deep nesting
   - Handle exceptions appropriately
   - Use comments sparingly; prefer self-documenting code

3. **DRY (Don't Repeat Yourself)**
   - Extract common code into utility methods
   - Use inheritance and composition appropriately
   - Avoid duplicate logic

4. **Error Handling**
   - Catch specific exceptions, not generic `Exception`
   - Log errors with context
   - Provide meaningful error messages

### Example Error Handling:

```csharp
try
{
    var product = await _repository.GetByIdAsync(id);
    if (product == null)
    {
        _logger.LogWarning("Product with ID {ProductId} not found", id);
        throw new NotFoundException($"Product with ID {id} not found");
    }
    return product;
}
catch (NotFoundException ex)
{
    _logger.LogError(ex, "Error retrieving product");
    throw;
}
```

## Testing

### Unit Tests

Write unit tests for new features:

```csharp
[TestClass]
public class ProductServiceTests
{
    private ProductService _service;
    private Mock<IProductRepository> _mockRepository;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepository.Object, new NullLogger<ProductService>());
    }

    [TestMethod]
    public async Task GetProductAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var expectedProduct = new Product { Id = productId, Name = "Test Product" };
        _mockRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _service.GetProductAsync(productId);

        // Assert
        Assert.AreEqual(expectedProduct, result);
        _mockRepository.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }
}
```

### Running Tests

```bash
dotnet test
```

## Documentation

### Code Comments

Use XML documentation comments for public members:

```csharp
/// <summary>
/// Retrieves a product by its ID.
/// </summary>
/// <param name="id">The unique identifier of the product</param>
/// <returns>The product if found; otherwise null</returns>
/// <exception cref="NotFoundException">Thrown when product is not found</exception>
public async Task<Product> GetProductAsync(Guid id)
{
    // Implementation
}
```

### README & Docs

- Update README.md if adding new features
- Document architectural changes in `docs/ARCHITECTURE.md`
- Add API documentation for new endpoints in `docs/API.md`
- Update setup instructions if dependencies change

## Pull Request Process

1. **Update** your branch with the latest `develop` branch
   ```bash
   git fetch origin
   git rebase origin/develop
   ```

2. **Run Tests**
   ```bash
   dotnet test
   ```

3. **Build Project**
   ```bash
   dotnet build
   ```

4. **Push Changes**
   ```bash
   git push origin feature/your-feature
   ```

5. **Create Pull Request**
   - Use a clear title describing the changes
   - Reference related issues (#123)
   - Include description of what changed and why
   - Link to any related documentation

### PR Template

```markdown
## Description
Brief description of the changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Related Issues
Closes #123

## Testing
- [ ] Added unit tests
- [ ] Tested locally
- [ ] No new warnings

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings generated
```

## Reporting Issues

### Bug Reports

Include:
- Clear description of the bug
- Steps to reproduce
- Expected behavior
- Actual behavior
- Environment information (.NET version, OS, etc.)
- Error messages and logs

### Feature Requests

Include:
- Clear description of the feature
- Use case and motivation
- Possible implementation approach
- Any related issues or discussions

## Development Environment Tips

### Debugging with Visual Studio

1. Set breakpoints by clicking line numbers
2. Press F5 to start debugging
3. Use Debug menu for additional windows (Watch, Locals, Call Stack)

### Database Migrations

When modifying database schema:
```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# View migration status
dotnet ef migrations list
```

### Code Review Checklist

Before submitting a PR, ensure:

- [ ] Code is clean and follows conventions
- [ ] No hardcoded values or sensitive data
- [ ] Proper error handling implemented
- [ ] Logging added where appropriate
- [ ] Tests written and passing
- [ ] No breaking changes without discussion
- [ ] Documentation updated
- [ ] Commit messages are clear

## Questions?

- Open an issue on GitHub for questions about contributing
- Check existing issues and discussions
- Contact maintainers if needed

Thank you for contributing to Market MVC!
