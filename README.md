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




The backend follows a modular monolith style, where each business module contains its own controllers, DTOs, models, interfaces, and services.

Backend Modules
Auth Module

Handles:

Register
Login
JWT generation
User roles
Books Module

Handles:

Public book listing
Public book details
Admin book creation
Admin book editing
Admin book deletion
Orders Module

Handles:

Creating orders
Viewing user orders
Viewing all orders as admin
Updating order status as admin
API Endpoints
Auth
POST /api/auth/register
POST /api/auth/login
Books

Public:

GET /api/books
GET /api/books/{id}

Admin only:

POST /api/books
PUT /api/books/{id}
DELETE /api/books/{id}
Orders

Customer:

POST /api/orders
GET /api/orders/my-orders

Admin only:

GET /api/orders/all
PUT /api/orders/{id}/status
Order Status Values
Pending
Processing
Shipped
Delivered
Cancelled
Running with Docker

The backend and PostgreSQL can run using Docker Compose.

1. Create .env

Copy the example file:

copy .env.example .env

Then update the values if needed.

Example:

POSTGRES_DB=OnlineBookStoreDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

JWT_KEY=change_me_to_a_long_random_secret
JWT_ISSUER=OnlineBookStore.Api
JWT_AUDIENCE=OnlineBookStore.Client
JWT_EXPIRES_IN_MINUTES=60

API_PORT=5177
2. Start Docker
docker compose up --build

The API will be available at:

http://localhost:5177

Test:

http://localhost:5177/api/books
3. Stop Docker
docker compose down

This keeps the database data.

To delete the database volume and start fresh:

docker compose down -v

Use -v carefully because it deletes PostgreSQL data.

Running Angular Frontend

The Angular frontend runs locally during development.

From the Angular project folder:

npm install
ng serve

Then open:

http://localhost:4200

Angular calls the backend API at:

http://localhost:5177
Database and Migrations

The API applies migrations automatically on startup:

dbContext.Database.Migrate();

It also seeds initial books if the Books table is empty.

Admin User Setup

After registering a user, you can manually update the role to admin in PostgreSQL:

UPDATE "Users"
SET "Role" = 'Admin'
WHERE "Email" = 'your-email@example.com';

Then login again to receive a new JWT containing the Admin role.

If using Docker PostgreSQL, connect with:

docker exec -it online-bookstore-postgres psql -U postgres -d OnlineBookStoreDb
Secrets

Do not commit real secrets.

The project uses:

.env

for Docker secrets.

The .env file should not be committed to GitHub.

Use:

.env.example

as a safe template.

For local Visual Studio development, use User Secrets for sensitive values such as:

Jwt:Key
ConnectionStrings:DefaultConnection
Error Handling

Unexpected backend errors return a friendly message to the frontend.

The backend logs the real exception with an internal request ID so the error can be traced in logs without exposing technical details to users.

Development Notes

Recommended development workflow:

Visual Studio → backend debugging
VS Code → Angular frontend
Docker → PostgreSQL and deployment-like backend testing

When backend code changes and you are running through Docker, rebuild the container:

docker compose up --build
