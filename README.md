# 🏢 Shared Database Multi-Tenancy

This repository demonstrates a production-ready shared database multi-tenancy solution for ASP.NET Core using [Finbuckle.MultiTenant](https://github.com/Finbuckle/Finbuckle.MultiTenant). For implementation details, see this [blog post](https://medium.com/@zahidcakici/multitenancy-and-finbukcle-in-net-f1d5e7e5f1bf).

## 🛠️ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools)
- IDE
  - JetBrains Rider
  - Visual Studio
  - Visual Studio Code

## 🚀 Getting Started

Follow these steps to set up and run the project:

1. Configure Connection Strings

   Open `appsettings.json` in the `SharedDatabase.Api` and `MigrationService.Host` projects and update the connection strings if needed:

   ```json
   {
     "ConnectionStrings": {
       "Database": "Server=.\\SQLEXPRESS;Initial Catalog=multitenancy;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=60;"
     }
   }
   ```
   Make sure they match for both services.

2. Run `MigrationService.Host`

   This service will create the database if it doesn't exist, apply migrations, and seed data. It will exit automatically when complete.

3. Run `Identity.Api`

   This service is used for JWT token generation.

   - Navigate to Swagger UI
   - Use the `POST /token` endpoint with this request body:
   ```json
   {
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "test@example.com",
      "customClaims": {
         "http://schemas.microsoft.com/identity/claims/tenantid": "tenant1"
      }
   }
   ```
   - Copy the JWT token from the response

4. Run `SharedDatabase.Api`
   - Navigate to Swagger UI
   - Click "Authorize" at the top and paste your JWT token
   - Try out the `GET /api/companies` endpoint
   - You'll only see data specific to `tenant1`

5. Test With Different Tenants
   - Generate a new JWT token with a different tenant identifier (`tenant2` instead of `tenant1`)
   - Authorize with the new token
   - Notice you get different data for `tenant2`

## 🔔 Real-time Notifications with SignalR

The repository includes a multi-tenant SignalR implementation that respects tenant boundaries when broadcasting messages.

### Getting Started with SignalR

1. Configure URLs

   Open `appsettings.json` in the `Blazor.Web` project and configure the SignalR and `Identity.Api` URLs:

   ```json
   {
     "SignalR": {
       "HubUrl": "http://localhost:5224/notifications"
     },
     "Identity": {
       "Url": "http://localhost:5132"
     }
   }
   ```
   Make sure these URLs match your service endpoints.

2. Run all three services
   - `Identity.Api`
   - `SharedDatabase.Api`
   - `Blazor.Web`

3. Navigate to the Notifications page
   - Open `Blazor.Web` in your browser
   - Navigate to the Notifications page

4. Test Notifications
   - Click the "Send" button to request JSON serialized company data for your tenant
   - You should see a list of companies belonging to your tenant appear in the messages list

## Migrations

### Create Migration

```bash
# For application data
dotnet ef migrations add Init \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
    --context AppDbContext \
    --output-dir Persistence/Migrations

# For tenant metadata
dotnet ef migrations add AddTenantMetadata \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
    --context TenantDbContext \
    --output-dir MultiTenancy/Migrations
```

### Apply Migrations

```bash
# For application data
dotnet ef database update \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
    --context AppDbContext

# For tenant metadata
dotnet ef database update \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
    --context TenantDbContext
```

## Running the Project in Docker with HTTPS

To run the project locally in Docker with HTTPS, follow these steps:

1. Generate and Trust the Development Certificate

   Before starting the application, generate and trust the development SSL certificate:

   - Generate the certificate:
     ```bash
     dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p VerySecurePassword123@!
     ```

   - Trust the certificate:
     ```bash
     dotnet dev-certs https --trust
     ```

2. Build and Run the Project

   Use `docker-compose` to build and run the project with the following command:
   ```bash
   docker-compose up -d
   ```

   The application will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:44362`

3. Volume Mapping for the Certificate

   Ensure that the development certificate is properly mounted into the Docker container by having this volume mapping in the `docker-compose.yml` file:
   ```yaml
   volumes:
     - ${USERPROFILE}\.aspnet\https:/https/:ro
   ```

This will allow the ASP.NET Core Web API to use HTTPS in the development environment within Docker.
