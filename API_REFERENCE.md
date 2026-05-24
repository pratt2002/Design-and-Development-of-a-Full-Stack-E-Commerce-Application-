# E-Commerce API Reference Guide
## Documentation Originality

This document is written specifically for this project implementation. To keep similarity low and maintain academic integrity, adapt examples, screenshots, metrics, and wording to your own execution results before final submission.

---

## Base URL
```
http://localhost:5119/api
```

---

## ðŸ“Œ Authentication Endpoints

### 1. User Login
```http
POST /auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}
```

**Response (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "id": 1,
  "username": "admin",
  "email": "admin@ecommerce.com",
  "role": "Admin",
  "firstName": "Admin",
  "lastName": "User"
}
```

**Error (401)**:
```json
{
  "message": "Invalid username or password"
}
```

---

### 2. User Registration
```http
POST /auth/register
Content-Type: application/json

{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "SecurePass123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "id": 2,
  "username": "newuser",
  "email": "newuser@example.com",
  "role": "Customer",
  "message": "User registered successfully"
}
```

**Error (400)**:
```json
{
  "message": "Username already exists"
}
```

---

### 3. Verify Token
```http
POST /auth/verify-token
Authorization: Bearer {token}
```

**Response (200 OK)**:
```json
{
  "message": "Token is valid"
}
```

---

## ðŸ›ï¸ Products Endpoints

### 1. Get All Products (with Filters)
```http
GET /products
GET /products?search=laptop&category=Electronics&minPrice=100&maxPrice=1000&pageNumber=1&pageSize=12
```

**Query Parameters**:
- `search` (string): Search by name or description
- `category` (string): Filter by category name
- `minPrice` (decimal): Minimum price filter
- `maxPrice` (decimal): Maximum price filter
- `pageNumber` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 12)

**Response (200 OK)**:
```json
{
  "totalCount": 45,
  "pageNumber": 1,
  "pageSize": 12,
  "products": [
    {
      "id": 1,
      "name": "Laptop",
      "description": "High-performance laptop",
      "price": 999.99,
      "stock": 5,
      "category": "Electronics",
      "imageUrl": "https://example.com/laptop.jpg",
      "discount": null,
      "isActive": true,
      "createdAt": "2024-05-14T10:00:00",
      "updatedAt": "2024-05-14T10:00:00"
    }
  ]
}
```

---

### 2. Get Single Product
```http
GET /products/1
```

**Response (200 OK)**:
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 999.99,
  "stock": 5,
  "category": "Electronics",
  "imageUrl": "https://example.com/laptop.jpg"
}
```

---

### 3. Create Product (Admin Only)
```http
POST /products
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "New Laptop",
  "description": "Latest model",
  "price": 1299.99,
  "stock": 10,
  "category": "Electronics",
  "imageUrl": "https://example.com/new-laptop.jpg",
  "sku": "LAPTOP-2024-001"
}
```

**Response (201 Created)**:
```json
{
  "id": 3,
  "name": "New Laptop",
  "price": 1299.99,
  "stock": 10
}
```

---

### 4. Update Product (Admin Only)
```http
PUT /products/1
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "Updated Laptop",
  "price": 899.99,
  "stock": 8,
  "isActive": true
}
```

**Response (200 OK)**:
```json
{
  "message": "Product updated successfully",
  "product": { ... }
}
```

---

### 5. Delete Product (Admin Only)
```http
DELETE /products/1
Authorization: Bearer {admin-token}
```

**Response (200 OK)**:
```json
{
  "message": "Product deleted successfully"
}
```

---

## ðŸ·ï¸ Categories Endpoints

### 1. Get All Categories
```http
GET /categories
```

**Response (200 OK)**:
```json
[
  {
    "id": 1,
    "name": "Electronics",
    "description": "Electronic devices",
    "isActive": true,
    "displayOrder": 1
  }
]
```

---

### 2. Create Category (Admin Only)
```http
POST /categories
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "Clothing",
  "description": "Apparel and accessories",
  "displayOrder": 2
}
```

---

## ðŸ›’ Shopping Cart Endpoints

### 1. Get User Cart
```http
GET /cart/1
Authorization: Bearer {token}
```

**Response (200 OK)**:
```json
{
  "cartItems": [
    {
      "id": 1,
      "productId": 1,
      "quantity": 2,
      "userId": 1,
      "product": {
        "id": 1,
        "name": "Laptop",
        "price": 999.99
      }
    }
  ],
  "totalItems": 1,
  "totalAmount": 1999.98
}
```

---

### 2. Add Item to Cart
```http
POST /cart/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

**Response (200 OK)**:
```json
{
  "message": "Item added to cart successfully"
}
```

---

### 3. Update Cart Item Quantity
```http
PATCH /cart/1/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "quantity": 3
}
```

---

### 4. Remove Item from Cart
```http
DELETE /cart/1/1
Authorization: Bearer {token}
```

---

### 5. Clear Entire Cart
```http
DELETE /cart/1
Authorization: Bearer {token}
```

---

## ðŸ“¦ Orders Endpoints

### 1. Place New Order
```http
POST /orders/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "shippingAddress": "123 Main St, City, State 12345",
  "paymentMethod": "Credit Card"
}
```

**Response (201 Created)**:
```json
{
  "orderId": 5,
  "totalAmount": 1999.98,
  "status": "Pending",
  "paymentRequired": true
}
```

---

### 2. Get Order Details
```http
GET /orders/5
Authorization: Bearer {token}
```

**Response (200 OK)**:
```json
{
  "id": 5,
  "userId": 1,
  "totalAmount": 1999.98,
  "status": "Processing",
  "shippingAddress": "123 Main St, City, State 12345",
  "trackingNumber": null,
  "createdAt": "2024-05-14T12:00:00",
  "orderItems": [
    {
      "id": 1,
      "productId": 1,
      "quantity": 2,
      "price": 999.99,
      "product": { "name": "Laptop" }
    }
  ],
  "payment": {
    "id": 5,
    "status": "Completed",
    "amount": 1999.98
  }
}
```

---

### 3. Get User's Orders
```http
GET /orders/user/1
Authorization: Bearer {token}
```

**Response (200 OK)**:
```json
[
  {
    "id": 5,
    "totalAmount": 1999.98,
    "status": "Processing",
    "createdAt": "2024-05-14T12:00:00"
  }
]
```

---

### 4. Update Order Status (Admin Only)
```http
PUT /orders/5/status
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "status": "Shipped",
  "trackingNumber": "TRK123456789"
}
```

**Valid Statuses**:
- Pending
- Processing
- Shipped
- Delivered
- Cancelled

---

### 5. Get All Orders (Admin Only)
```http
GET /orders
Authorization: Bearer {admin-token}
GET /orders?status=Processing&pageNumber=1&pageSize=20
```

**Response (200 OK)**:
```json
{
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 20,
  "orders": [ ... ]
}
```

---

## ðŸ’³ Payment Endpoints

### 1. Process Payment
```http
POST /payment/process
Authorization: Bearer {token}
Content-Type: application/json

{
  "orderId": 5,
  "cardNumber": "4242424242424242",
  "cardholderName": "John Doe",
  "expiry": "12/25",
  "cvv": "123"
}
```

**Response (200 OK)**:
```json
{
  "message": "Payment processed successfully",
  "transactionId": "TXN_ABC123DEF456",
  "status": "Completed",
  "amount": 1999.98
}
```

---

### 2. Get Payment Status
```http
GET /payment/5
Authorization: Bearer {token}
```

**Response (200 OK)**:
```json
{
  "orderId": 5,
  "amount": 1999.98,
  "status": "Completed",
  "paymentMethod": "Credit Card",
  "cardLastFour": "4242",
  "processedAt": "2024-05-14T12:05:00"
}
```

---

### 3. Refund Payment (Admin Only)
```http
POST /payment/refund
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "orderId": 5,
  "reason": "Customer requested cancellation"
}
```

**Response (200 OK)**:
```json
{
  "message": "Payment refunded successfully",
  "amount": 1999.98
}
```

---

## ðŸ”’ Authorization

### Token Format
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Claims
- `NameIdentifier`: User ID
- `Name`: Username
- `Role`: User role (Admin/Customer)

### Role-Based Access
| Endpoint | Requires | Roles |
|----------|----------|-------|
| GET /products | âŒ | Public |
| POST /products | âœ… | Admin |
| PUT /products/{id} | âœ… | Admin |
| DELETE /products/{id} | âœ… | Admin |
| GET /cart/{userId} | âœ… | Owner/Admin |
| POST /orders/{userId} | âœ… | Owner/Admin |
| PUT /orders/{id}/status | âœ… | Admin |
| POST /payment/process | âœ… | Owner/Admin |
| POST /payment/refund | âœ… | Admin |

---

## ðŸ“Š Error Codes

| Code | Status | Meaning |
|------|--------|---------|
| 200 | OK | Successful request |
| 201 | Created | Resource created successfully |
| 400 | Bad Request | Invalid request parameters |
| 401 | Unauthorized | Missing or invalid token |
| 403 | Forbidden | Access denied |
| 404 | Not Found | Resource not found |
| 422 | Unprocessable | Validation error |
| 500 | Server Error | Internal server error |

---

## ðŸ§ª cURL Examples

### Login
```bash
curl -X POST http://localhost:5119/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}'
```

### Get Products
```bash
curl http://localhost:5119/api/products?search=laptop&pageSize=5
```

### Add to Cart (with token)
```bash
curl -X POST http://localhost:5119/api/cart/1 \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"productId":1,"quantity":2}'
```

### Create Order
```bash
curl -X POST http://localhost:5119/api/orders/1 \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"shippingAddress":"123 Main St"}'
```

---

## ðŸ“ Request/Response Format

All requests and responses use JSON format.

### Standard Error Response
```json
{
  "message": "Error description",
  "error": "Detailed error information (if applicable)"
}
```

### Pagination Response
```json
{
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 10,
  "items": [ ... ]
}
```

---

**Last Updated**: May 2024

