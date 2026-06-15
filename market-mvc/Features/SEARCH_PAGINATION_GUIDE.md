# Search, Pagination, Filter & Sort Guide

This guide explains how to use the centralized search, pagination, filtering, and sorting infrastructure across the application.

## Overview

All search-related functionality has been centralized in `Infrastructure/Common/` with reusable components:

- **SearchSpecification<T>** - Base class for search queries
- **SearchRequest** - DTO for search parameters
- **ISearchService** - Service for search operations
- **FilterOptions** - Advanced filtering
- **Result<T>** - Standardized response wrapper

Feature-specific implementations:
- **ProductSearchSpecification** - For product queries
- **OrderSearchSpecification** - For order queries

## Basic Usage

### 1. Create a Feature-Specific Specification

Location: `Features/[Feature]/Specifications/[Entity]SearchSpecification.cs`

```csharp
public class ProductSearchSpecification : SearchSpecification<Product>
{
    public ProductSearchSpecification(
        string searchTerm = "",
        int pageNumber = 1,
        int pageSize = 10,
        string orderBy = "Name",
        string orderDirection = "asc",
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Guid? categoryId = null,
        string? priceRange = null)
    {
        SearchTerm = searchTerm;
        PageNumber = pageNumber;
        PageSize = pageSize;
        OrderBy = orderBy;
        OrderDirection = orderDirection;

        ValidatePaginationParameters();

        // Build filters
        BuildSearchFilter();
        BuildPriceFilter();
        BuildCategoryFilter();

        // Apply ordering
        ApplyOrdering(GetOrderExpression());

        // Apply pagination
        ApplyPaging();

        // Include relations
        AddInclude(p => p.Category);
    }

    private void BuildSearchFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return;

        var term = SearchTerm.Trim().ToLower();
        ApplySearch(p => 
            p.Name.ToLower().Contains(term) ||
            p.Description.ToLower().Contains(term)
        );
    }

    private void BuildPriceFilter()
    {
        if (MinPrice.HasValue)
            AddCriteria(p => p.Price >= MinPrice);
        
        if (MaxPrice.HasValue)
            AddCriteria(p => p.Price <= MaxPrice);
    }

    private Expression<Func<Product, dynamic>> GetOrderExpression()
    {
        return OrderBy?.ToLower() switch
        {
            "price" => p => p.Price,
            "name" => p => p.Name,
            _ => p => p.Name
        };
    }
}
```

### 2. Update CQRS Query

Location: `Features/[Feature]/Queries/GetProductsQuery.cs`

```csharp
public class GetProductsQuery : IRequest<PaginatedResult<ProductCardVM>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "Name";
    public string SortOrder { get; set; } = "asc";
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public Guid? CategoryId { get; set; }
    public string? PriceRange { get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;
    }
}
```

### 3. Update Query Handler

Location: `Features/[Feature]/Queries/GetProducts/GetProductsQueryHandler.cs`

```csharp
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResult<ProductCardVM>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public async Task<PaginatedResult<ProductCardVM>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            request.Validate();

            // Create specification with all filters
            var specification = new ProductSearchSpecification(
                searchTerm: request.SearchTerm,
                pageNumber: request.Page,
                pageSize: request.PageSize,
                orderBy: request.SortBy,
                orderDirection: request.SortOrder,
                minPrice: request.MinPrice,
                maxPrice: request.MaxPrice,
                categoryId: request.CategoryId,
                priceRange: request.PriceRange
            );

            // Get paginated results
            var products = await _unitOfWork.Products.GetAsync(specification, cancellationToken);
            var total = await _unitOfWork.Products.CountAsync(specification, cancellationToken);

            // Map to view models
            var mapped = _mapper.Map<List<ProductCardVM>>(products);

            _logger.LogInformation(
                "Retrieved {Count} products (Page {Page} of {Total})",
                products.Count,
                request.Page,
                total
            );

            return new PaginatedResult<ProductCardVM>
            {
                Items = mapped,
                TotalItems = total,
                CurrentPage = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid search parameters");
            throw new ValidationException("Search parameters invalid", "INVALID_SEARCH_PARAMS");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            throw new DataAccessException("Failed to retrieve products", ex);
        }
    }
}
```

### 4. Update Controller

Location: `Controllers/ProductsController.cs`

```csharp
[HttpGet]
public async Task<IActionResult> Index(SearchRequest request)
{
    try
    {
        // Validate request
        request.Validate();

        // Create query from search request
        var query = new GetProductsQuery
        {
            SearchTerm = request.SearchTerm,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.OrderBy,
            SortOrder = request.OrderDirection
        };

        // Handle AJAX requests differently
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var result = await _mediator.Send(query);
            return Json(new
            {
                html = await this.RenderViewAsync("_ProductList", result.Items),
                totalItems = result.TotalItems,
                currentPage = result.CurrentPage,
                totalPages = result.TotalPages
            });
        }

        // Regular request
        var products = await _mediator.Send(query);
        
        return Ok(Result<PaginatedResult<ProductCardVM>>.SuccessResult(
            products,
            "Products retrieved successfully"
        ));
    }
    catch (ValidationException ex)
    {
        return UnprocessableEntity(Result<object>.ValidationFailure(ex.Failures));
    }
    catch (DataAccessException ex)
    {
        return StatusCode(500, Result<object>.FailureResult(ex.Message, 500));
    }
}
```

## Advanced Features

### Custom Filters

```csharp
public class CustomSearchSpecification : SearchSpecification<Product>
{
    public CustomSearchSpecification(string search)
    {
        SearchTerm = search;

        // Single filter
        ApplySearch(p => p.Name.Contains(search));

        // Multiple filters combined with AND
        AddCriteria(p => p.IsActive);
        AddCriteria(p => p.Price > 0);

        // Date range
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        AddCriteria(p => p.CreatedAt >= thirtyDaysAgo);

        ApplyOrdering(p => p.Name);
        ApplyPaging();
    }
}
```

### Range Filters

```csharp
var priceFilter = new RangeFilter(min: 100, max: 500);

if (priceFilter.IsValid())
{
    specification.AddCriteria(p => 
        p.Price >= priceFilter.Min && 
        p.Price <= priceFilter.Max
    );
}
```

### Date Range Filters

```csharp
var dateRange = new DateRangeFilter(
    startDate: DateTime.Now.AddMonths(-1),
    endDate: DateTime.Now
);

if (dateRange.IsValid())
{
    specification.AddCriteria(o => 
        o.CreatedAt >= dateRange.StartDate &&
        o.CreatedAt <= dateRange.EndDate
    );
}
```

### Advanced Filter Options

```csharp
var filters = new FilterOptions()
    .AddFilter("Status", OrderStatus.Pending)
    .AddFilter("MinAmount", 100)
    .AddFilter("MaxAmount", 500)
    .IncludeField("OrderNumber")
    .IncludeField("Total");

// Check if filter exists
if (filters.HasFilter("Status"))
{
    var status = filters.GetFilter("Status");
    // Apply filter
}
```

## Common Patterns

### Search Multiple Fields

```csharp
private void BuildSearchFilter()
{
    if (string.IsNullOrWhiteSpace(SearchTerm))
        return;

    var term = SearchTerm.Trim().ToLower();
    ApplySearch(p =>
        p.Name.ToLower().Contains(term) ||
        p.Description.ToLower().Contains(term) ||
        p.Sku.ToLower().Contains(term) ||
        p.Tags.ToLower().Contains(term) ||
        (p.Category != null && p.Category.Name.ToLower().Contains(term))
    );
}
```

### Dynamic Sorting

```csharp
private Expression<Func<Product, dynamic>> GetOrderExpression()
{
    return OrderBy?.ToLower() switch
    {
        "price" => p => p.Price,
        "name" => p => p.Name,
        "popularity" => p => p.SortOrder,
        "createdat" => p => p.CreatedAt,
        "rating" => p => p.Reviews.Average(r => r.Rating),
        _ => p => p.Name
    };
}
```

### Conditional Includes

```csharp
public ProductSearchSpecification(bool includeReviews = false)
{
    AddInclude(p => p.Category);
    
    if (includeReviews)
    {
        AddInclude(p => p.Reviews);
    }
}
```

## Error Handling

### Validation Errors

```csharp
if (request.Page < 1)
    throw new ValidationException(
        new Dictionary<string, string[]>
        {
            { "Page", new[] { "Page number must be greater than 0" } }
        }
    );
```

### Not Found

```csharp
var products = await repository.GetAsync(spec);
if (!products.Any())
    throw new NotFoundException("Products", "matching the search criteria");
```

### Business Rule Violations

```csharp
if (request.PageSize > 100)
    throw new BusinessRuleException("Page size cannot exceed 100 items");
```

## View Examples

### Basic Search Form

```html
<form method="get" action="/Products" class="search-form">
    <input type="text" name="searchTerm" placeholder="Search products..." />
    
    <select name="orderBy">
        <option value="name">Name</option>
        <option value="price">Price</option>
        <option value="createdat">Newest</option>
    </select>

    <select name="orderDirection">
        <option value="asc">Ascending</option>
        <option value="desc">Descending</option>
    </select>

    <input type="hidden" name="pageNumber" value="1" />
    <input type="hidden" name="pageSize" value="10" />

    <button type="submit">Search</button>
</form>
```

### Pagination Display

```html
@if (Model.HasPreviousPage)
{
    <a href="?pageNumber=@(Model.CurrentPage - 1)">Previous</a>
}

@for (var i = 1; i <= Model.TotalPages; i++)
{
    @if (i == Model.CurrentPage)
    {
        <span>@i</span>
    }
    else
    {
        <a href="?pageNumber=@i">@i</a>
    }
}

@if (Model.HasNextPage)
{
    <a href="?pageNumber=@(Model.CurrentPage + 1)">Next</a>
}
```

### AJAX Search

```javascript
function searchProducts() {
    const formData = new FormData(document.querySelector('.search-form'));
    
    fetch('/Products', {
        method: 'GET',
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.json())
    .then(data => {
        document.querySelector('.product-list').innerHTML = data.html;
        updatePagination(data.currentPage, data.totalPages);
    });
}
```

## Performance Tips

1. **Limit Page Size** - Maximum 100 items per page to prevent memory exhaustion

2. **Eager Load Relations** - Use `AddInclude()` to load related entities

3. **Indexed Columns** - Ensure search fields are indexed in the database

4. **Search Term Normalization** - Use `ISearchService.NormalizeSearchTerm()` for consistency

5. **Caching** - Cache popular searches or categories

```csharp
// Cache example
var cacheKey = $"products-{request.SearchTerm}-{request.Page}";
if (_cache.TryGetValue(cacheKey, out PaginatedResult<Product> cached))
{
    return cached;
}

var result = await handler.Handle(request, cancellationToken);
_cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
return result;
```

## Testing Examples

### Unit Test

```csharp
[Test]
public void ProductSearchSpecification_BuildsCorrectFilters()
{
    // Arrange
    var spec = new ProductSearchSpecification(
        searchTerm: "iPhone",
        pageNumber: 2,
        pageSize: 20,
        minPrice: 100,
        maxPrice: 500
    );

    // Assert
    Assert.That(spec.PageNumber, Is.EqualTo(2));
    Assert.That(spec.PageSize, Is.EqualTo(20));
    Assert.That(spec.Criteria, Is.Not.Null);
    Assert.That(spec.GetSkip(), Is.EqualTo(20)); // (2-1) * 20
}
```

### Integration Test

```csharp
[Test]
public async Task GetProductsQuery_WithSearchTerm_ReturnsMatchingProducts()
{
    // Arrange
    var query = new GetProductsQuery
    {
        SearchTerm = "iPhone",
        Page = 1,
        PageSize = 10
    };

    // Act
    var result = await handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.That(result.Items, Is.Not.Empty);
    Assert.That(result.Items.All(p => p.Name.Contains("iPhone")), Is.True);
}
```

## See Also

- `Infrastructure/Common/README.md` - Infrastructure documentation
- `Features/Products/Specifications/ProductSearchSpecification.cs` - Product implementation
- `Features/Orders/Specifications/OrderSearchSpecification.cs` - Order implementation
- `Domain/PaginatedResult.cs` - Pagination response model
