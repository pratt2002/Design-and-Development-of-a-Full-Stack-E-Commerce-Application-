# E-Commerce Platform - Complete System Architecture and Documentation

## Documentation Originality

This document is tailored to this repository's implementation. Before submission, replace placeholders, add your own screenshots, and include your own execution evidence to maintain academic integrity.

---

## Project Overview

This is a **full-stack e-commerce platform** with the following features:

- **Product Management**: Browse, search, filter products by category and price
- **User Authentication**: JWT-based authentication with role-based access
- **Shopping Cart**: Add/remove items, manage quantities
- **Order Management**: Place orders, track order status
- **Payment Processing**: Mock payment gateway integration
- **Admin Dashboard**: Manage products, categories, and orders
- **Security**: Password hashing (BCrypt), JWT tokens, authorization checks, SQL injection prevention

---

## System Architecture

### Frontend (React)
- **Location**: `/ecommerce-ui/`
- **Port**: 3000
- **Key Libraries**: Axios, React Hooks
- **Pages**: Shop, Checkout, AdminDashboard, Login/Register
- **Features**: Real-time search, filtering, pagination

### Backend (.NET Core)
- **Location**: `/EcommerceAPI/`
- **Port**: 5000
- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 7.0.18

---

## ðŸ“Š Database Schema

### Models & Relationships

```
User
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ Username (Unique)
â”œâ”€â”€ Email (Unique)
â”œâ”€â”€ Password (Hashed)
â”œâ”€â”€ FirstName, LastName
â”œâ”€â”€ Role (Customer/Admin)
â”œâ”€â”€ Address, PhoneNumber
â”œâ”€â”€ CreatedAt, UpdatedAt
â”œâ”€â”€ Relationships:
    â”œâ”€â”€ Orders [1:Many]
    â””â”€â”€ CartItems [1:Many]

Product
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ Name
â”œâ”€â”€ Description
â”œâ”€â”€ Price (decimal 18,2)
â”œâ”€â”€ Stock
â”œâ”€â”€ Category
â”œâ”€â”€ SKU (Unique, Optional)
â”œâ”€â”€ ImageUrl
â”œâ”€â”€ Discount (Optional)
â”œâ”€â”€ IsActive
â”œâ”€â”€ CreatedAt, UpdatedAt
â”œâ”€â”€ Relationships:
    â”œâ”€â”€ OrderItems [1:Many]
    â””â”€â”€ CartItems [1:Many]

Order
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ UserId (FK)
â”œâ”€â”€ TotalAmount (decimal 18,2)
â”œâ”€â”€ Status (Pending/Processing/Shipped/Delivered/Cancelled)
â”œâ”€â”€ ShippingAddress
â”œâ”€â”€ TrackingNumber (Optional)
â”œâ”€â”€ CreatedAt, UpdatedAt
â”œâ”€â”€ DeliveredAt (Optional)
â”œâ”€â”€ Relationships:
    â”œâ”€â”€ User [Many:1]
    â”œâ”€â”€ OrderItems [1:Many]
    â””â”€â”€ Payment [1:1]

OrderItem
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ OrderId (FK)
â”œâ”€â”€ ProductId (FK)
â”œâ”€â”€ Quantity
â”œâ”€â”€ Price (at time of purchase)
â”œâ”€â”€ Discount (Optional)
â”œâ”€â”€ CreatedAt
â”œâ”€â”€ Relationships:
    â”œâ”€â”€ Order [Many:1]
    â””â”€â”€ Product [Many:1]

CartItem
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ UserId (FK)
â”œâ”€â”€ ProductId (FK)
â”œâ”€â”€ Quantity
â”œâ”€â”€ AddedAt
â”œâ”€â”€ Relationships:
    â”œâ”€â”€ User [Many:1]
    â””â”€â”€ Product [Many:1]

Payment
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ OrderId (FK, Unique)
â”œâ”€â”€ Amount (decimal 18,2)
â”œâ”€â”€ Status (Pending/Completed/Failed/Refunded)
â”œâ”€â”€ PaymentMethod
â”œâ”€â”€ StripeTransactionId (Optional)
â”œâ”€â”€ CardLastFour (Optional)
â”œâ”€â”€ CreatedAt, UpdatedAt
â”œâ”€â”€ ProcessedAt (Optional)
â”œâ”€â”€ Relationships:
    â””â”€â”€ Order [1:1]

Category
â”œâ”€â”€ Id (PK)
â”œâ”€â”€ Name
â”œâ”€â”€ Description
â”œâ”€â”€ Icon (Optional)
â”œâ”€â”€ IsActive
â”œâ”€â”€ DisplayOrder
â”œâ”€â”€ CreatedAt
```

---

## ðŸ” Security Features

### 1. **Password Hashing**
- **Library**: BCrypt.Net-Next
- **Workfactor**: 12 (industry standard)
- **Implementation**: Passwords hashed before storage

### 2. **JWT Authentication**
```
Token Structure:
- Claims: UserId, Username, Role
- Algorithm: HS256
- Secret Key: Configured in appsettings.json
- Expiration: 24 hours (configurable)
```

### 3. **Authorization**
- **Role-based**: Admin, Customer
- **Controllers protected** with `[Authorize]` attribute
- **User isolation**: Users can only see their own orders/carts

### 4. **Input Validation**
- Email format validation
- Password complexity requirements
- Price and quantity validation
- XSS prevention through parameter validation

### 5. **Database Security**
- SQL injection prevention through EF Core parameterized queries
- Unique constraints on sensitive fields (username, email, SKU)
- Foreign key constraints with cascade delete

---

## ðŸ”— API Endpoints

### Authentication
```
POST   /api/auth/login                 - Login user
POST   /api/auth/register              - Register new user
POST   /api/auth/verify-token          - Verify JWT token
```

### Products
```
GET    /api/products                   - Get products (with pagination, search, filter)
GET    /api/products/{id}              - Get single product
GET    /api/products/category/{name}   - Get products by category
POST   /api/products                   - Create product [Admin]
PUT    /api/products/{id}              - Update product [Admin]
DELETE /api/products/{id}              - Delete product [Admin]
```

### Categories
```
GET    /api/categories                 - Get all categories
GET    /api/categories/{id}            - Get single category
POST   /api/categories                 - Create category [Admin]
PUT    /api/categories/{id}            - Update category [Admin]
DELETE /api/categories/{id}            - Delete category [Admin]
```

### Cart
```
GET    /api/cart/{userId}              - Get user's cart
POST   /api/cart/{userId}              - Add item to cart
PATCH  /api/cart/{userId}/{productId}  - Update cart item quantity
DELETE /api/cart/{userId}/{productId}  - Remove item from cart
DELETE /api/cart/{userId}              - Clear entire cart
```

### Orders
```
POST   /api/orders/{userId}            - Place new order
GET    /api/orders/{id}                - Get order details
GET    /api/orders/user/{userId}       - Get user's orders
PUT    /api/orders/{id}/status         - Update order status [Admin]
GET    /api/orders                     - Get all orders [Admin]
```

### Payment
```
POST   /api/payment/process            - Process payment
GET    /api/payment/{orderId}          - Get payment status
POST   /api/payment/refund             - Refund payment [Admin]
```

---

## ðŸš€ Setup & Running the Application

### Prerequisites
- .NET 9 SDK
- Node.js 16+
- SQL Server / LocalDB
- Git

### Backend Setup

1. **Navigate to API directory**
   ```bash
   cd EcommerceAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (appsettings.json)
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EcommerceDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

4. **Run migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the API**
   ```bash
   dotnet run
   ```
   - API will be available at: `http://localhost:5119`
   - Swagger UI: `http://localhost:5119/swagger`

### Seed Demo Data

Add sample data to the database (optional):

```csharp
// In Program.cs, add after app.Build():
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    // Add demo users, products, categories
}
```

### Frontend Setup

1. **Navigate to UI directory**
   ```bash
   cd ecommerce-ui
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start development server**
   ```bash
   npm start
   ```
   - UI will be available at: `http://localhost:3000`

---

## ðŸ‘¨â€ðŸ’» Demo Credentials

### Admin Account
```
Username: admin
Password: password123
Email: admin@ecommerce.com
Role: Admin
```

### Customer Account
```
Username: customer
Password: password123
Email: customer@ecommerce.com
Role: Customer
```

> **Note**: Create these accounts by registering on the login page or through database seed.

---

## ðŸ§ª Testing

### API Testing with Swagger
1. Navigate to `http://localhost:5119/swagger`
2. Authorize with JWT token from login endpoint
3. Test all endpoints

### Manual Testing Steps

1. **User Registration**
   - Go to login page
   - Click "Register here"
   - Fill in details (min 8 char password)
   - Submit

2. **Product Browsing**
   - View all products on home page
   - Search by product name
   - Filter by category
   - Filter by price range
   - Pagination works (12 products per page)

3. **Shopping Cart**
   - Add products to cart
   - View cart summary
   - Update quantities
   - Remove items
   - Cart persists in localStorage

4. **Checkout**
   - Enter shipping address
   - Enter payment details (card number, expiry, CVV)
   - Process payment
   - View order confirmation

5. **Admin Dashboard** (if admin user)
   - View all orders
   - Update order status
   - Manage products (create, edit, delete)
   - View categories

---

## ðŸ“ Project Structure

```
Ecommerce/
â”œâ”€â”€ EcommerceAPI/
â”‚   â”œâ”€â”€ Controllers/
â”‚   â”‚   â”œâ”€â”€ AuthController.cs
â”‚   â”‚   â”œâ”€â”€ ProductsController.cs
â”‚   â”‚   â”œâ”€â”€ CartController.cs
â”‚   â”‚   â”œâ”€â”€ OrdersController.cs
â”‚   â”‚   â”œâ”€â”€ PaymentController.cs
â”‚   â”‚   â””â”€â”€ CategoriesController.cs
â”‚   â”œâ”€â”€ Models/
â”‚   â”‚   â”œâ”€â”€ User.cs
â”‚   â”‚   â”œâ”€â”€ Product.cs
â”‚   â”‚   â”œâ”€â”€ Cart.cs
â”‚   â”‚   â”œâ”€â”€ Order.cs
â”‚   â”‚   â”œâ”€â”€ OrderItem.cs
â”‚   â”‚   â”œâ”€â”€ Payment.cs
â”‚   â”‚   â””â”€â”€ Category.cs
â”‚   â”œâ”€â”€ Data/
â”‚   â”‚   â””â”€â”€ AppDbContext.cs
â”‚   â”œâ”€â”€ Services/
â”‚   â”‚   â”œâ”€â”€ JwtService.cs
â”‚   â”‚   â””â”€â”€ PasswordService.cs
â”‚   â”œâ”€â”€ Migrations/
â”‚   â”œâ”€â”€ appsettings.json
â”‚   â””â”€â”€ Program.cs
â”‚
â””â”€â”€ ecommerce-ui/
    â”œâ”€â”€ src/
    â”‚   â”œâ”€â”€ components/
    â”‚   â”‚   â”œâ”€â”€ Product.js
    â”‚   â”‚   â”œâ”€â”€ Cart.js
    â”‚   â”‚   â””â”€â”€ Navbar.js
    â”‚   â”œâ”€â”€ pages/
    â”‚   â”‚   â”œâ”€â”€ Login.js
    â”‚   â”‚   â”œâ”€â”€ Checkout.js
    â”‚   â”‚   â””â”€â”€ AdminDashboard.js
    â”‚   â”œâ”€â”€ App.js
    â”‚   â”œâ”€â”€ App.css
    â”‚   â””â”€â”€ index.js
    â”œâ”€â”€ package.json
    â”œâ”€â”€ public/
    â””â”€â”€ README.md
```

---

## ðŸ”„ Key Features Breakdown

### 1. Product Management
- Full CRUD operations
- Search by name/description
- Filter by category and price
- Stock management
- Discount support
- Pagination

### 2. User Management
- Secure registration with validation
- JWT-based authentication
- Role-based authorization (Admin/Customer)
- Password hashing with BCrypt
- Profile management

### 3. Shopping Cart
- Add/remove products
- Modify quantities
- Stock availability check
- Persistent storage (localStorage)
- Cart summary with total

### 4. Orders & Checkout
- Multi-step checkout (Shipping â†’ Payment â†’ Confirmation)
- Order creation with items
- Order tracking with status
- Order history for users
- Admin order management

### 5. Payment Processing
- Mock Stripe integration
- Payment status tracking
- Transaction logging
- Refund capability
- Card details storage (last 4 digits only)

### 6. Admin Dashboard
- Order management with status updates
- Product CRUD operations
- Category management
- Real-time inventory tracking
- Analytics-ready structure

---

## ðŸ›¡ï¸ Common Security Considerations

### âœ… Implemented
- Password hashing (BCrypt)
- JWT token validation
- CORS policy configuration
- Input validation
- SQL injection prevention (EF Core)
- Authorization checks on sensitive endpoints

### ðŸ”„ Best Practices
1. **Never store plain passwords** - Always use bcrypt or equivalent
2. **HTTPS in production** - Current setup uses HTTP for demo
3. **Secure token storage** - Use httpOnly cookies (not localStorage)
4. **Rate limiting** - Implement for authentication endpoints
5. **Audit logging** - Log all admin actions
6. **Data encryption** - Encrypt sensitive fields in database

---

## ðŸ“¦ Dependencies

### Backend
- Microsoft.EntityFrameworkCore 7.0.18
- Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0
- System.IdentityModel.Tokens.Jwt 8.0.2
- BCrypt.Net-Next 4.0.3
- Swashbuckle.AspNetCore 6.5.0

### Frontend
- react 19.2.5
- react-dom 19.2.5
- axios 1.15.2
- react-scripts 5.0.1

---

## ðŸ› Troubleshooting

### Issue: "Connection refused" when connecting to API
**Solution**: Ensure API is running on port 5000
```bash
cd EcommerceAPI
dotnet run
```

### Issue: JWT token expired
**Solution**: Login again to get a new token (default: 24 hour expiration)

### Issue: Products not loading
**Solution**: Check if database migrations were applied
```bash
dotnet ef database update
```

### Issue: Admin access not working
**Solution**: Ensure you're logged in as admin user (Role = "Admin")

---

## ðŸ“ˆ Future Enhancements

1. Real Stripe payment integration
2. Email notifications
3. Product reviews and ratings
4. Wishlist functionality
5. Order cancellation by customers
6. Advanced analytics dashboard
7. Inventory alerts
8. Multi-currency support
9. Product recommendations
10. Two-factor authentication

---

## ðŸ“ž Support & Questions

For issues or questions:
1. Check the troubleshooting section above
2. Review API endpoint documentation
3. Check browser console for errors
4. Review backend logs in terminal

---

## ðŸ“ License

This project is for educational purposes.

---

**Last Updated**: May 2024
**Version**: 1.0.0

