# Online Book Store

A fullstack online book store application built with **.NET Web API**, **Angular**, and **PostgreSQL**.

The project supports customer book browsing, authentication, cart checkout, order tracking, admin book management, admin order status updates, structured logging, validation, and Docker setup.

---

## Tech Stack

### Backend
- .NET Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Role-based Authorization
- Serilog Logging
- Docker

### Frontend
- Angular
- TypeScript
- Angular Routing
- HttpClient
- Auth Guard
- HTTP Interceptors
- LocalStorage Cart

### Database
- PostgreSQL

---

## Main Features

### Customer Features
- Register and login
- Browse books
- View book details
- Add books to cart
- Update cart quantity
- Checkout
- View personal orders
- Track order status

### Admin Features
- Add books
- Edit books
- Delete books
- Update book stock
- View all orders
- View customer name and email for each order
- Update order status

### Security Features
- JWT authentication
- Role-based authorization
- Admin-only endpoints
- Token expiry handling in Angular
- Secrets moved out of source code using `.env` and User Secrets

### Logging and Error Handling
- Global exception middleware
- Serilog logging
- Hidden request ID for internal error tracing
- Angular global error banner

---

## Project Structure

```text
OnlineBookStore.Api
│
├── Modules
│   ├── Auth
│   ├── Books
│   └── Orders
│
├── Shared
│   ├── Data
│   ├── Helpers
│   ├── Middleware
│   └── Enums
│
├── Dockerfile
├── docker-compose.yml
├── .env.example
└── README.md
