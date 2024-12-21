# Multi-Tenancy

This is a simple implementation of shared database multi-tenancy using the [Finbuckle.MultiTenant](https://github.com/Finbuckle/Finbuckle.MultiTenant) library.

## Prerequisites

Ensure the following tools are installed:
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools) (`dotnet ef`)

## Migrations

### 1. Create Migration (Persistence Layer)

Run the following command to add a migration for the persistence layer (`AppDbContext`):

```bash
dotnet ef migrations add Init \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase \
    --context AppDbContext \
    --output-dir Persistence/Migrations
```

This will create a migration for the main persistence layer where your application's data resides.

### 2. Apply Migration (Persistence Layer)

Run the following command to apply the migration to the database:

```bash
dotnet ef database update \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase \
    --context AppDbContext
```

This will apply all pending migrations to the database.

### 3. Create Migration (Tenant Metadata)

Run the following command to add a migration for the tenant metadata (`TenantDbContext`):

```bash
dotnet ef migrations add AddTenantMetadata \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase \
    --context TenantDbContext \
    --output-dir MultiTenancy/Migrations
```

This will create a migration for the `TenantDbContext`, which contains metadata about tenants.

> **Note:** The `TenantDbContext` should be stored in a separate database if we're using the **database per tenant** approach. The tenant metadata cannot reside in a tenant-specific database.

### 4. Apply Migration (Tenant Metadata)

Run the following command to apply the migration for the tenant metadata:

```bash
dotnet ef database update \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase \
    --context TenantDbContext
```

This will apply the tenant metadata migration to its respective database.

## Getting Started with the Project

To run the project locally in Docker with HTTPS, follow these steps:

1. **Generate and Trust the Development Certificate:**

   Before starting the application, generate and trust the development SSL certificate:

   - **Generate the certificate:**
     ```bash
     dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p VerySecurePassword123@!
     ```

   - **Trust the certificate:**
     ```bash
     dotnet dev-certs https --trust
     ```

2. **Build and Run the Project:**

   Use `docker-compose` to build and run the project with the following command:
   ```bash
   docker-compose up -d
   ```

   The application will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:44362`

3. **Volume Mapping for the Certificate:**

   Ensure that the development certificate is properly mounted into the Docker container by having this volume mapping in the `docker-compose.yml` file:
   ```yaml
   volumes:
     - ${USERPROFILE}\.aspnet\https:/https/:ro
   ```

This will allow the ASP.NET Core Web API to use HTTPS in the development environment within Docker.
