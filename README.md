# Integration Health & Incident Dashboard

## Overview
This demo showcases enterprise integration monitoring and incident response workflows using a .NET 8 backend, PostgreSQL, and a React + TypeScript frontend. It is organized using clean architecture and emphasizes operational support, reliability, and maintainable design.

## Architecture
- `backend/src/Dashboard.Api`: ASP.NET Core Web API with Swagger
- `backend/src/Dashboard.Application`: application services and DTOs
- `backend/src/Dashboard.Domain`: domain entities and enums
- `backend/src/Dashboard.Infrastructure`: EF Core, PostgreSQL, health check runner
- `frontend`: React + TypeScript (Vite)
- `docker-compose.yml`: PostgreSQL container definition

## Prerequisites
- .NET 8 SDK
- Node.js 20+
- Docker Desktop

## Environment Configuration
This project avoids committed secrets and connection strings. Configure locally using environment variables.

Backend:
- Set `ConnectionStrings__DefaultConnection` to your PostgreSQL connection string.
- Example format: `Host=<host>;Port=<port>;Database=<db>;Username=<user>;Password=<password>`

Frontend:
- Set `VITE_API_BASE_URL` to the backend API base URL (for example, `http://localhost:5000`).
- You can place this in a local `.env.local` file (not committed).

Docker (PostgreSQL):
- Set `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD` before running `docker-compose up`.

## Run Locally
1. Start PostgreSQL:
   - `docker-compose up -d`
2. Apply migrations:
   - `dotnet tool install --global dotnet-ef`
   - `dotnet ef database update --project backend/src/Dashboard.Infrastructure --startup-project backend/src/Dashboard.Api`
3. Start the API:
   - `dotnet run --project backend/src/Dashboard.Api`
4. Start the frontend:
   - `cd frontend`
   - `npm install`
   - `npm run dev`

Swagger is available at `http://localhost:5000/swagger` by default.

## API Endpoints
- `GET /api/services`
- `POST /api/checks/run`
- `GET /api/incidents?status=open`
- `POST /api/incidents`
- `PUT /api/incidents/{id}/status`
- `PUT /api/incidents/{id}/resolve`

## Testing
- `dotnet test backend/Dashboard.sln`

## How It Supports Enterprise Workflows
- Centralized visibility into service health and degradation
- Incident capture, status tracking, and resolution notes for audit trails
- On-demand checks with deterministic status mapping
- Clean architecture for maintainability and separation of concerns
