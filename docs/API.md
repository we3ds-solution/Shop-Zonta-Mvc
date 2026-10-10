# API Documentation

## Overview

The Market MVC application provides RESTful APIs for e-commerce operations including products, orders, customers, and more.

## Base URL

- **Development**: `http://localhost:5000/api`
- **Production**: `https://your-domain.com/api`

## Authentication

Currently, the application uses session-based authentication via ASP.NET Core Identity.

### Login
```http
POST /auth/login
Content-Type: application/x-www-form-urlencoded

username=user@example.com&password=password123
```

### Logout
```http
POST /auth/logout
```

## Endpoints

### Products

#### Get All Products
```http
GET /api/products?page=1&pageSize=10
```

**Response:**
```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Product Name",
      "description": "Product description",
      "price": 99.99,
      "stock": 100,
      "status": "Active"
    }
  ],
  "total": 50,
  "page": 1,
  "pageSize": 10
}
```

#### Get Product by ID
```http
GET /api/products/{id}
```

**Response:**
```json
{
  "id": "uuid",
  "name": "Product Name",
  "description": "Product description",
  "price": 99.99,
  "stock": 100,
  "categoryId": "uuid",
  "status": "Active",
  "imageUrl": "https://...",
  "createdAt": "2026-06-15T00:00:00Z"
}
```

#### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "New Product",
  "description": "Product description",
  "price": 99.99,
  "stock": 100,
  "categoryId": "uuid"
}
```

**Response:** `201 Created` with product object

#### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Product",
  "description": "Updated description",
  "price": 109.99,
  "stock": 150
}
```

**Response:** `200 OK` with updated product

#### Delete Product
```http
DELETE /api/products/{id}
```

**Response:** `204 No Content`

### Orders

#### Get All Orders
```http
GET /api/orders?page=1&pageSize=10
```

**Response:**
```json
{
  "data": [
    {
      "id": "uuid",
      "customerId": "uuid",
      "orderDate": "2026-06-15T00:00:00Z",
      "totalAmount": 299.99,
      "status": "Pending",
      "paymentStatus": "Pending"
    }
  ],
  "total": 100,
  "page": 1,
  "pageSize": 10
}
```

#### Get Order by ID
```http
GET /api/orders/{id}
```

**Response:**
```json
{
  "id": "uuid",
  "customerId": "uuid",
  "orderDate": "2026-06-15T00:00:00Z",
  "totalAmount": 299.99,
  "status": "Confirmed",
  "paymentStatus": "Completed",
  "items": [
    {
      "productId": "uuid",
      "productName": "Product Name",
      "quantity": 2,
      "unitPrice": 99.99,
      "totalPrice": 199.98
    }
  ]
}
```

#### Create Order
```http
POST /api/orders
Content-Type: application/json

{
  "customerId": "uuid",
  "items": [
    {
      "productId": "uuid",
      "quantity": 2
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Your City",
    "state": "State",
    "zipCode": "12345",
    "country": "Country"
  }
}
```

**Response:** `201 Created` with order object

#### Update Order Status
```http
PUT /api/orders/{id}/status
Content-Type: application/json

{
  "status": "Shipped"
}
```

**Response:** `200 OK`

### Customers

#### Get All Customers
```http
GET /api/customers?page=1&pageSize=10
```

#### Get Customer by ID
```http
GET /api/customers/{id}
```

#### Create Customer
```http
POST /api/customers
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "address": {
    "street": "123 Main St",
    "city": "Your City",
    "state": "State",
    "zipCode": "12345",
    "country": "Country"
  }
}
```

#### Update Customer
```http
PUT /api/customers/{id}
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Doe",
  "phone": "+0987654321"
}
```

### Categories

#### Get All Categories
```http
GET /api/categories
```

**Response:**
```json
[
  {
    "id": "uuid",
    "name": "Category Name",
    "description": "Category description",
    "imageUrl": "https://..."
  }
]
```

#### Get Category by ID
```http
GET /api/categories/{id}
```

#### Create Category
```http
POST /api/categories
Content-Type: application/json

{
  "name": "New Category",
  "description": "Category description"
}
```

#### Update Category
```http
PUT /api/categories/{id}
Content-Type: application/json

{
  "name": "Updated Category",
  "description": "Updated description"
}
```

#### Delete Category
```http
DELETE /api/categories/{id}
```

## Error Responses

All errors follow this format:

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": []
  }
}
```

### HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK - Request succeeded |
| 201 | Created - Resource created successfully |
| 204 | No Content - Resource deleted successfully |
| 400 | Bad Request - Invalid parameters |
| 401 | Unauthorized - Authentication required |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource not found |
| 409 | Conflict - Resource conflict |
| 500 | Internal Server Error - Server error |

### Common Error Codes

- `VALIDATION_ERROR` - Request validation failed
- `NOT_FOUND` - Resource not found
- `UNAUTHORIZED` - User not authenticated
- `FORBIDDEN` - User lacks permissions
- `CONFLICT` - Resource conflict (e.g., duplicate)
- `INTERNAL_ERROR` - Unexpected server error

## Pagination

All list endpoints support pagination:

**Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Items per page

**Response:**
```json
{
  "data": [],
  "total": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

## Filtering & Sorting

Some endpoints support filtering and sorting:

**Query Parameters:**
- `search` - Search query
- `sortBy` - Field to sort by
- `sortOrder` - `asc` or `desc`
- `status` - Filter by status
- `category` - Filter by category

## Rate Limiting

Currently not implemented. Will be added in future versions.

## CORS

CORS is configured for development. Update `Program.cs` for production:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## Testing with cURL

### Get All Products
```bash
curl -X GET "http://localhost:5000/api/products?page=1&pageSize=10" \
  -H "Content-Type: application/json"
```

### Create Product
```bash
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Product",
    "price": 99.99,
    "stock": 100,
    "categoryId": "uuid"
  }'
```

## Testing with Postman

Import the Postman collection:
1. Download `postman-collection.json` from the project root
2. Open Postman
3. Click "Import"
4. Select the JSON file

## Webhooks

Webhooks for order events will be available in future versions.

---

For Swagger/OpenAPI documentation, visit `/swagger` when available.
