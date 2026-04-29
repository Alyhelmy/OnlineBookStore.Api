# OnlineBookStore.Api

ASP.NET Core Web API for an online bookstore with modular structure for books and authentication.

## Tech Stack

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Authentication

## Getting Started

### Prerequisites

- .NET SDK 9.0+
- SQL Server instance

### Run Locally

1. Restore dependencies:
   - `dotnet restore`
2. Apply migrations:
   - `dotnet ef database update --project OnlineBookStore.Api`
3. Run the API:
   - `dotnet run --project OnlineBookStore.Api`

The API uses settings from:

- `OnlineBookStore.Api/appsettings.json`
- `OnlineBookStore.Api/appsettings.Development.json`

## Project Structure

- `OnlineBookStore.Api/Modules/Books` - book features
- `OnlineBookStore.Api/Modules/Auth` - authentication features
- `OnlineBookStore.Api/Shared` - shared infrastructure and utilities

## API Endpoints

Base URL (local): `https://localhost:<port>/api`

### Auth

- `POST /api/auth/register`
  - Registers a new user.
  - Body: `RegisterRequest`
  - Responses: `200 OK` on success, `400 Bad Request` on validation/business failure.
- `POST /api/auth/login`
  - Intended login endpoint using `LoginRequest`.
  - Note: the `Login` action exists in code but is not currently decorated with an HTTP attribute, so this route is not exposed yet.

### Books

- `GET /api/books`
  - Returns all books.
  - Response: `200 OK` with a list of books.
- `GET /api/books/{id}`
  - Returns one book by ID.
  - Responses: `200 OK` when found, `404 Not Found` when the book does not exist.

## Notes

- Keep secrets (connection strings, JWT keys) out of source control.
- Update configuration values per environment before deployment.
