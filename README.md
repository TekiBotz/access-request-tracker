# Access Request Tracker

ASP.NET Core Web API for tracking resource access requests. A user submits a request for access to a resource, an admin approves or denies it, and every decision is written to an audit trail.

## What it does

- Submit an access request (who, what resource, why). It starts as **Pending**.
- List requests, optionally filtered by status.
- Look up a single request with its full history.
- Approve or deny a request as an admin. The status changes and an audit row is written.

## Stack

- **C# / ASP.NET Core Web API** (.NET 10)
- **Entity Framework Core** with migrations
- **SQL Server** (LocalDB) locally, **Azure SQL Database** in the cloud
- **Azure App Service** for hosting
- **Azure DevOps Pipelines** for CI/CD (build, test, deploy)
- **xUnit** for tests
- **Bicep** for infrastructure as code, **Docker** for containerization

## How it's structured

- `Models/` — the three entities: `Resource`, `AccessRequest`, `AuditEntry`.
- `Data/AppDbContext.cs` — the EF Core DbContext; each `DbSet` maps to a table.
- `Controllers/RequestsController.cs` — the endpoints for submit, list, get, approve, and deny.
- `Migrations/` — EF Core migrations that build the database schema.
- `AccessTracker.Tests/` — xUnit tests against an in-memory database.
- `infra/main.bicep` — the Azure resources (App Service, SQL server and database) described as code.
- `Dockerfile` — a multi-stage build that packages the app into a container.
- `azure-pipelines.yml` — the CI/CD pipeline.

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| POST | `/api/requests` | Submit a new request (starts Pending) |
| GET | `/api/requests` | List requests (`?status=Pending` to filter) |
| GET | `/api/requests/{id}` | One request with its audit history |
| POST | `/api/requests/{id}/approve?adminName=` | Approve a request |
| POST | `/api/requests/{id}/deny?adminName=` | Deny a request |

## Running it locally

1. Open the solution in Visual Studio (needs the .NET 10 SDK).
2. The connection string in `appsettings.json` points at SQL Server LocalDB.
3. Apply the migrations from the Package Manager Console:
   ```
   Update-Database
   ```
4. Press F5. The Swagger page at `/swagger` lets you try each endpoint.

There's also an `AccessTracker.http` file with example requests you can send straight from Visual Studio.

## Running it in a container

```
docker build -t accesstracker .
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="<your connection string>" accesstracker
```

Then open `http://localhost:8080/swagger`. The connection string is passed as an environment variable, so no secrets live in the image.

## Deployment

The app is deployed to Azure App Service. A push to `main` triggers the Azure DevOps pipeline, which installs the SDK, restores, builds, runs the tests, publishes, and deploys. The database connection string is stored in App Service configuration, not in source control.

## Notes

- The database password and the Azure connection string are never committed. Locally the connection string uses a password-free LocalDB connection; in Azure the real string lives in App Service settings.
- The app enables EF Core's retry-on-failure because the free-tier Azure SQL database pauses when idle, so transient "waking up" errors are retried instead of surfacing to the caller.
