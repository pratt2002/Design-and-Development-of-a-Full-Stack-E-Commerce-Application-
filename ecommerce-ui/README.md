# Ecommerce UI (React Client)

## Documentation Originality

This README is customized for the current e-commerce implementation in this repository. Update screenshots, local URLs, and any environment notes before submission so the document reflects your own run.

---

## Purpose

This application is the customer and admin-facing frontend for the e-commerce platform. It communicates with the ASP.NET Core API and provides:

- Login and registration flow
- Product browsing with search and filters
- Cart interaction and checkout steps
- Role-based admin dashboard access

## Runtime Configuration

- Frontend URL: http://localhost:3000
- Backend API URL: http://localhost:5119/api

If your API port changes, update API base URL references in source files.

## Run in Development

```bash
npm install
npm start
```

## Build for Production

```bash
npm run build
```

## Key Frontend Modules

- `src/pages/Login.js`: authentication UI (login/register)
- `src/components/Product.js`: product list, search, and filtering
- `src/components/Cart.js`: cart display and remove behavior
- `src/pages/Checkout.js`: shipping, payment, confirmation steps
- `src/pages/AdminDashboard.js`: orders/products/categories admin operations
- `src/App.js`: top-level app state and route/view switching

## Testing Checklist

1. Login with customer account
2. Add product to cart
3. Complete checkout flow
4. Login with admin account
5. Update order status from admin dashboard

## Notes for Academic Submission

- Include UI screenshots for each core flow.
- Match endpoint URLs with backend run logs.
- Mention known limitations (mock payment, no recommendation engine).
