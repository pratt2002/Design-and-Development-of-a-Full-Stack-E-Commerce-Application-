# E-Commerce Platform - Quick Start Guide
## Documentation Originality

This document is written specifically for this project implementation. To keep similarity low and maintain academic integrity, adapt examples, screenshots, metrics, and wording to your own execution results before final submission.

---

## ðŸš€ 5-Minute Setup

### Step 1: Backend Setup (2 minutes)

Open PowerShell/Terminal and run:

```powershell
# Navigate to API folder
cd EcommerceAPI

# Restore dependencies
dotnet restore

# Run migrations (creates database)
dotnet ef database update

# Start the API
dotnet run
```

âœ… API running at: `http://localhost:5119`
âœ… Swagger UI at: `http://localhost:5119/swagger`

---

### Step 2: Frontend Setup (2 minutes)

Open a new PowerShell/Terminal window:

```bash
# Navigate to UI folder
cd ecommerce-ui

# Install dependencies
npm install

# Start the application
npm start
```

âœ… App opens at: `http://localhost:3000`

---

### Step 3: First Login (1 minute)

On the login page, use:

**Demo Admin Account**
```
Username: admin
Password: password123
```

or **Demo Customer Account**
```
Username: customer  
Password: password123
```

> **If accounts don't exist**: Register new account on Login page (min 8-char password)

---

## ðŸŽ¯ What to Try First

### As a Customer:
1. âœ… Browse products with search and filters
2. âœ… Add items to cart
3. âœ… Checkout (shipping â†’ payment â†’ confirmation)
4. âœ… View order history
5. âœ… Track order status

### As an Admin:
1. âœ… Click "Admin Dashboard" button in navbar
2. âœ… View all orders and update status
3. âœ… Create/Edit/Delete products
4. âœ… Manage categories
5. âœ… View inventory

---

## ðŸ“Š Key Features to Explore

### Shopping Features
- **Search**: Type in search box â†’ see results instantly
- **Filters**: Filter by category, price range
- **Pagination**: Navigate through product pages
- **Cart**: Add/remove items, see total
- **Checkout**: 3-step process with payment

### Admin Features
- **Order Management**: Update order status, track shipments
- **Product CRUD**: Create, read, update, delete products
- **Inventory**: Real-time stock tracking
- **Analytics**: Order summary and status reports

---

## ðŸ’¡ Demo Data

### Pre-populated Data:
- Admin user (see above)
- Sample products in database
- Sample categories

### Add More Data:
1. Go to Admin Dashboard
2. Click "+ Add Product" button
3. Fill in product details
4. Save and see it appear in catalog

---

## ðŸ” Testing Payment

Use demo card: `4242 4242 4242 4242`

During checkout:
- Card Number: `4242 4242 4242 4242`
- Name: Any name
- Expiry: `12/25` (any future date)
- CVV: `123` (any 3 digits)

---

## âš ï¸ Common Issues & Solutions

### "Can't connect to API"
```
âŒ Problem: API not running
âœ… Solution: Run "dotnet run" in EcommerceAPI folder
```

### "Database error"
```
âŒ Problem: Migrations not applied
âœ… Solution: Run "dotnet ef database update"
```

### "Products not showing"
```
âŒ Problem: No products in database
âœ… Solution: Create products in Admin Dashboard
```

### "JWT token error"
```
âŒ Problem: Token expired
âœ… Solution: Logout and login again
```

### "Can't access Admin Dashboard"
```
âŒ Problem: Not logged in as admin
âœ… Solution: Login with admin account (username: admin)
```

---

## ðŸ› ï¸ Project Structure

```
Ecommerce/
â”œâ”€â”€ EcommerceAPI/          â† Backend (.NET)
â”‚   â”œâ”€â”€ Controllers/
â”‚   â”œâ”€â”€ Models/
â”‚   â”œâ”€â”€ Services/
â”‚   â”œâ”€â”€ Data/
â”‚   â”œâ”€â”€ appsettings.json
â”‚   â””â”€â”€ Program.cs
â”‚
â””â”€â”€ ecommerce-ui/          â† Frontend (React)
    â”œâ”€â”€ src/
    â”œâ”€â”€ public/
    â”œâ”€â”€ package.json
    â””â”€â”€ README.md
```

---

## ðŸ“š Documentation Files

- **SYSTEM_ARCHITECTURE.md**: Complete system design & architecture
- **API_REFERENCE.md**: All API endpoints with examples
- **This file**: Quick start guide

---

## ðŸ” Security

- âœ… Passwords hashed with BCrypt
- âœ… JWT token authentication
- âœ… Role-based authorization
- âœ… Input validation
- âœ… SQL injection prevention

---

## ðŸŽ“ Learning Points

This project demonstrates:

1. **Full-Stack Development**: React frontend + .NET backend
2. **REST API Design**: Proper endpoint structure
3. **Database Design**: Relational schema with EF Core
4. **Authentication**: JWT implementation
5. **Authorization**: Role-based access control
6. **Best Practices**: Security, error handling, validation
7. **UI/UX**: Responsive design, user interactions
8. **Admin Dashboard**: Order/inventory management

---

## ðŸš€ Next Steps

### To Deploy:
1. Update database connection string for production
2. Change JWT secret key
3. Deploy API to Azure/AWS/Heroku
4. Deploy UI to Netlify/Vercel
5. Update API URLs in frontend

### To Extend:
1. Add email notifications
2. Implement real Stripe integration
3. Add product reviews
4. Add wishlist feature
5. Implement advanced searches
6. Add analytics dashboard

---

## ðŸ“ž Quick Reference

| What | Where | How |
|------|-------|-----|
| **API Docs** | Swagger UI | http://localhost:5119/swagger |
| **Backend Code** | EcommerceAPI/ | C# .NET Core |
| **Frontend Code** | ecommerce-ui/src/ | React JavaScript |
| **Database** | LocalDB | SQL Server |
| **Login** | http://localhost:3000 | admin/password123 |
| **Admin Panel** | React App | Click "Admin Dashboard" |

---

## âœ… Verification Checklist

After setup, verify:

- [ ] Backend running on port 5000
- [ ] Frontend running on port 3000
- [ ] Can login with demo account
- [ ] Products display on home page
- [ ] Can add items to cart
- [ ] Can checkout
- [ ] Admin dashboard accessible (if admin user)
- [ ] Can create/edit products (if admin)

All checked? âœ… **You're ready to go!**

---

**Enjoy building with this e-commerce platform!** ðŸŽ‰

For detailed information, see:
- [System Architecture](./SYSTEM_ARCHITECTURE.md)
- [API Reference](./API_REFERENCE.md)

