# Centralized Infrastructure

This directory contains centralized cross-cutting concerns for the application:
- **Error Handling** - Standardized exception hierarchy and handling
- **Search/Filter/Pagination** - Reusable components for data queries
- **Result Wrapping** - Consistent response objects

## Error Handling

### Exception Hierarchy

All custom exceptions inherit from `ApplicationException`:

```
ApplicationException (base)
├── NotFoundException
├── ValidationException
├── UnauthorizedException
├── ForbiddenException
├── ConflictException
│   └── DuplicateException
├── BusinessRuleException
└── DataAccessException
```

### Usage Examples

#### Throwing Exceptions

```csharp
// Not found
throw new NotFoundException("Product", productId);
// Output: "Product with ID '123' was not found"

// Validation error
throw new ValidationException(
    new Dictionary<string, string[]>
    {
        { "Email", new[] { "Invalid email format" } },
        { "Password", new[] { "Too weak", "Must contain number" } }
    }
);

// Business rule violation
throw new BusinessRuleException("Order cannot be shipped - payment not received");

// Duplicate entry
throw new DuplicateException("User", "Email", "john@example.com");

// Unauthorized
throw new UnauthorizedException("Admin access required");

// Access denied
throw new ForbiddenException("You cannot modify other users' profiles");
```

#### Middleware Handling

The `ExceptionHandlingMiddleware` automatically catches exceptions and returns appropriate HTTP responses:

```csharp
// In middleware
context.Response.StatusCode = StatusCodes.Status404NotFound; // NotFoundException
context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity; // ValidationException
context.Response.StatusCode = StatusCodes.Status401Unauthorized; // UnauthorizedException
context.Response.StatusCode = StatusCodes.Status403Forbidden; // ForbiddenException
context.Response.StatusCode = StatusCodes.Status409Conflict; // ConflictException
context.Response.StatusCode = StatusCodes.Status400BadRequest; // Other custom
context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Unexpected
```

## Result Wrapping

### Result<T> Generic Class

Standardized response wrapper for services:

```csharp
// Success response
var result = Result<User>.SuccessResult(
    data: user,
    message: "User created successfully",
    statusCode: 201
);

// Failure response
var result = Result<User>.FailureResult("User not found", 404);

// Validation failure
var result = Result<User>.ValidationFailure(
    new Dictionary<string, string[]>
    {
        { "Email", new[] { "Email already exists" } }
    }
);

// Not found
var result = Result<User>.NotFound("User with ID 123 not found");

// Unauthorized
var result = Result<User>.Unauthorized();

// Forbidden
var result = Result<User>.Forbidden("Cannot access other user profiles");
```

### Result Non-Generic Class

For void operations:

```csharp
var result = Result.SuccessResult("Order saved successfully");
var result = Result.FailureResult("Failed to save order");
var result = Result.NotFound("Order not found");
```

## Search, Filter, Pagination, Sorting

### SearchSpecification<T>

Base class for building search queries with common functionality:

```csharp
public class ProductSearchSpecification : SearchSpecification<Product>
{
    public ProductSearchSpecification(string searchTerm, int pageNumber, int pageSize, string orderBy, string orderDirection)
    {
        SearchTerm = searchTerm;
        PageNumber = pageNumber;
        PageSize = pageSize;
        OrderBy = orderBy;
        OrderDirection = orderDirection;

        ValidatePaginationParameters();

        // Apply search filter
        ApplySearch(p => 
            p.Name.Contains(searchTerm) || 
            p.Description.Contains(searchTerm)
        );

        // Apply ordering
        ApplyOrdering(p => p.Name);

        // Apply pagination
        ApplyPaging();
    }
}
```

### SearchRequest DTO

Generic search request object:

```csharp
public class SearchRequest
{
    public string SearchTerm { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = string.Empty;
    public string OrderDirection { get; set; } = "asc";

    public void Validate()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;
    }
}
```

### FilterOptions

Advanced filtering:

```csharp
var filters = new FilterOptions()
    .AddFilter("Category", "Electronics")
    .AddFilter("MinPrice", 100)
    .AddFilter("MaxPrice", 500)
    .IncludeField("Name")
    .IncludeField("Price");

if (filters.HasFilter("Category"))
{
    var category = filters.GetFilter("Category");
}
```

### RangeFilter

For numeric ranges:

```csharp
var priceRange = new RangeFilter(100, 500);
if (priceRange.IsValid())
{
    // Apply filter
}
```

### DateRangeFilter

For date ranges:

```csharp
var dateRange = new DateRangeFilter(
    DateTime.Now.AddMonths(-1),
    DateTime.Now
);

if (dateRange.IsValid())
{
    // Apply filter
}
```

## SearchService

Centralized service for search operations:

```csharp
public interface ISearchService
{
    // Validate parameters
    void ValidateSearchParameters(int pageNumber, int pageSize, string orderBy);

    // Calculate pagination offset
    int CalculateSkip(int pageNumber, int pageSize);

    // Build sort expression
    string BuildSortExpression(string fieldName, string direction);

    // Normalize search term
    string NormalizeSearchTerm(string searchTerm);

    // Build SQL-like filter
    string BuildSearchFilter(string searchTerm, params string[] fields);

    // Get keywords from search term
    List<string> GetSearchKeywords(string searchTerm);
}
```

### Usage

```csharp
[Inject]
private readonly ISearchService _searchService;

public async Task<IActionResult> Search(SearchRequest request)
{
    request.Validate();
    _searchService.ValidateSearchParameters(request.PageNumber, request.PageSize, request.OrderBy);

    var skip = _searchService.CalculateSkip(request.PageNumber, request.PageSize);
    var keywords = _searchService.GetSearchKeywords(request.SearchTerm);
    var sortExpression = _searchService.BuildSortExpression(request.OrderBy, request.OrderDirection);

    // Apply to query
}
```

## Expression Extensions

Utilities for combining LINQ expressions:

```csharp
// AND two expressions
var combined = expr1.AndAlso(expr2);

// OR two expressions
var combined = expr1.OrElse(expr2);

// Negate expression
var negated = expr.Not();
```

## Integration Examples

### In Controllers

```csharp
[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] SearchRequest request)
{
    try
    {
        request.Validate();
        var result = await _mediator.Send(new GetProductsQuery
        {
            SearchTerm = request.SearchTerm,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.OrderBy,
            SortOrder = request.OrderDirection
        });

        return Ok(Result<PaginatedResult<ProductCardVM>>.SuccessResult(result));
    }
    catch (ValidationException ex)
    {
        return UnprocessableEntity(Result<object>.ValidationFailure(ex.Failures));
    }
    catch (NotFoundException ex)
    {
        return NotFound(Result<object>.NotFound(ex.Message));
    }
}
```

### In Handlers

```csharp
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResult<ProductCardVM>>
{
    public async Task<PaginatedResult<ProductCardVM>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page < 1)
            throw new ValidationException("PageNumber", "Page must be greater than 0");

        var specification = new ProductSearchSpecification(
            request.SearchTerm,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortOrder
        );

        var products = await _repository.GetAsync(specification);
        var total = await _repository.CountAsync(specification);

        var mapped = _mapper.Map<List<ProductCardVM>>(products);

        return new PaginatedResult<ProductCardVM>
        {
            Items = mapped,
            TotalItems = total,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };
    }
}
```

### In Services

```csharp
public async Task<Result<List<UserDTO>>> SearchUsers(string searchTerm, int page, int pageSize)
{
    try
    {
        var request = new SearchRequest
        {
            SearchTerm = searchTerm,
            PageNumber = page,
            PageSize = pageSize
        };
        request.Validate();

        _searchService.ValidateSearchParameters(request.PageNumber, request.PageSize, "");

        var keywords = _searchService.GetSearchKeywords(request.SearchTerm);
        
        var users = await _userRepository.SearchAsync(keywords, page, pageSize);
        
        return Result<List<UserDTO>>.SuccessResult(_mapper.Map<List<UserDTO>>(users));
    }
    catch (ValidationException ex)
    {
        return Result<List<UserDTO>>.FailureResult(ex.Message);
    }
}
```

## Best Practices

### 1. Always Throw Specific Exceptions

```csharp
// Good
if (user == null)
    throw new NotFoundException("User", userId);

// Avoid
if (user == null)
    throw new Exception("Not found");
```

### 2. Use SearchSpecification for Complex Queries

```csharp
// Good
var spec = new ProductSearchSpecification(searchTerm, page, size, sortBy, sortDir);
var results = await repository.GetAsync(spec);

// Avoid
var results = await context.Products
    .Where(p => p.Name.Contains(search))
    .OrderBy(p => p.Name)
    .Skip((page - 1) * size)
    .Take(size)
    .ToListAsync();
```

### 3. Validate Early

```csharp
// In handler
request.Validate();
if (!isValid)
    throw new ValidationException(errors);

// Continue processing
```

### 4. Use Result Wrapper in Services

```csharp
// Good
public async Task<Result<User>> GetUser(Guid id)
{
    var user = await _repo.GetByIdAsync(id);
    if (user == null)
        return Result<User>.NotFound();
    return Result<User>.SuccessResult(user);
}

// Avoid
public async Task<User> GetUser(Guid id)
{
    return await _repo.GetByIdAsync(id);
    // Caller doesn't know if null means error or no result
}
```

### 5. Log All Exceptions

The middleware automatically logs exceptions. In handlers/services:

```csharp
try
{
    // Process
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process {Operation}", nameof(MyOperation));
    throw new DataAccessException("Operation failed", ex);
}
```

## Performance Considerations

- **Max Page Size**: Limited to 100 to prevent memory exhaustion
- **Default Page Size**: 10 items per page
- **Search Term Normalization**: Removes extra spaces, converts to lowercase
- **Pagination Skip**: Calculated as `(PageNumber - 1) * PageSize`

## Testing

### Unit Test Example

```csharp
[Test]
public void SearchService_ValidateSearchParameters_ThrowsOnInvalidPage()
{
    // Arrange
    var searchService = new SearchService();

    // Act & Assert
    Assert.Throws<ArgumentException>(() =>
        searchService.ValidateSearchParameters(0, 10, "")
    );
}

[Test]
public void SearchService_NormalizeSearchTerm_ReturnsLowercase()
{
    // Arrange
    var searchService = new SearchService();
    
    // Act
    var result = searchService.NormalizeSearchTerm("  iPhone  Pro  ");
    
    // Assert
    Assert.AreEqual("iphone pro", result);
}
```

## Files Overview

- **Result.cs** - Result<T> and Result wrapper classes
- **Exceptions.cs** - Custom exception hierarchy
- **SearchSpecification.cs** - Base specification for search queries
- **ExpressionExtensions.cs** - LINQ expression utilities
- **SearchService.cs** - Centralized search operations
- **README.md** - This file

## Related Documentation

- See `Features/Common/BaseSpecification.cs` for specification base class
- See `Infrastructure/Middleware/ExceptionHandlingMiddleware.cs` for exception handling
- See individual feature handlers for implementation examples
