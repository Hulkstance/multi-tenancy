# Multi-Tenancy

This repository provides a sample implementation of shared database multi-tenancy in ASP.NET Core, utilizing the [Finbuckle.MultiTenant](https://github.com/Finbuckle/Finbuckle.MultiTenant) library in ASP.NET Core. For a detailed explanation and insights, check out this [blog post](https://medium.com/@zahidcakici/multitenancy-and-finbukcle-in-net-f1d5e7e5f1bf).

## Prerequisites

Ensure the following tools are installed:
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools) (`dotnet ef`)

## Getting Started with the Project

1. Launch both the `Identity.Api` and `SharedDatabase.Api` projects.
   - The `Identity.Api` service will be accessible at `https://localhost:7271`.
   - You can open the Swagger UI by navigating to `https://localhost:7271` in your browser.

2. Call the `POST /token` endpoint with the following payload:

```json
{
   "userId":"3fa85f64-5717-4562-b3fc-2c963f66afa6",
   "email":"test@example.com",
   "customClaims": {
      "http://schemas.microsoft.com/identity/claims/tenantid": "tenant1"
   }
}
```

3. Run migrations and update the database:

```bash
# Adds the initial migration for the persistence layer and generates migration files for AppDbContext.
dotnet ef migrations add Init --project src\SharedDatabase.Infrastructure --startup-project src\SharedDatabase.Api --context AppDbContext --output-dir Persistence\Migrations

# Updates the database with the migrations for the persistence layer.
dotnet ef database update --project src\SharedDatabase.Infrastructure --startup-project src\SharedDatabase.Api --context AppDbContext

# Adds a migration for tenant metadata and generates migration files for TenantDbContext.
dotnet ef migrations add AddTenantMetadata --project src\SharedDatabase.Infrastructure --startup-project src\SharedDatabase.Api --context TenantDbContext --output-dir MultiTenancy\Migrations

# Updates the database with the migrations for the tenant metadata.
dotnet ef database update --project src\SharedDatabase.Infrastructure --startup-project src\SharedDatabase.Api --context TenantDbContext
```

4. Insert the following seed data into the database:

```sql
INSERT INTO [MultiTenancy].[Tenants] ([Id], [Identifier], [Name])
VALUES
  ('f47ac10b-58cc-4372-a567-0e02b2c3d479', 'tenant1', 'Tenant One'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d480', 'tenant2', 'Tenant Two'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d481', 'tenant3', 'Tenant Three'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d482', 'tenant4', 'Tenant Four'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d483', 'tenant5', 'Tenant Five'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d484', 'tenant6', 'Tenant Six'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d485', 'tenant7', 'Tenant Seven'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d486', 'tenant8', 'Tenant Eight'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d487', 'tenant9', 'Tenant Nine'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d488', 'tenant10', 'Tenant Ten');

INSERT INTO [App].[Companies] ([Id], [Name], [TenantId])
VALUES
  ('d02d41be-3e42-4f34-9d10-bd4d1a803433', 'Company One', 'f47ac10b-58cc-4372-a567-0e02b2c3d479'),
  ('b0569c89-4c33-4c8b-95c6-3f46bc984b90', 'Company Two', 'f47ac10b-58cc-4372-a567-0e02b2c3d479'),
  ('b912c95f-2eb9-4a9d-9f7b-0a8e1e4f7d92', 'Company Three', 'f47ac10b-58cc-4372-a567-0e02b2c3d480'),
  ('8db1a120-b7a4-4e3f-b40f-3b1fc11fa6a1', 'Company Four', 'f47ac10b-58cc-4372-a567-0e02b2c3d480'),
  ('731c63c2-dc90-4026-ae90-f6b5997058d5', 'Company Five', 'f47ac10b-58cc-4372-a567-0e02b2c3d481'),
  ('9e12b2fd-df64-4b67-8f34-8d6d2b5a5275', 'Company Six', 'f47ac10b-58cc-4372-a567-0e02b2c3d481'),
  ('37c1cdd6-1329-47c5-b88e-b47c2e941aad', 'Company Seven', 'f47ac10b-58cc-4372-a567-0e02b2c3d482'),
  ('e4f640c1-cdf4-4a4a-b0f5-8b62f5d57ad5', 'Company Eight', 'f47ac10b-58cc-4372-a567-0e02b2c3d482'),
  ('af1adf73-82e6-4f61-b6bb-7faab3d2dbcc', 'Company Nine', 'f47ac10b-58cc-4372-a567-0e02b2c3d483'),
  ('3972bc2a-30e3-4ab5-9b89-55d2337e319b', 'Company Ten', 'f47ac10b-58cc-4372-a567-0e02b2c3d483'),
  ('46a4b925-c378-429d-9271-87aef764c482', 'Company Eleven', 'f47ac10b-58cc-4372-a567-0e02b2c3d484'),
  ('b8a26fa7-2185-409b-a618-819f52b3958d', 'Company Twelve', 'f47ac10b-58cc-4372-a567-0e02b2c3d484'),
  ('0e85d25e-c9d2-47d5-989f-e313ada6d5d1', 'Company Thirteen', 'f47ac10b-58cc-4372-a567-0e02b2c3d485'),
  ('5045bb02-550f-4a89-84f0-d47b7027cb67', 'Company Fourteen', 'f47ac10b-58cc-4372-a567-0e02b2c3d485'),
  ('f82bb13e-d7e9-4d3d-b3bb-94052f97a72c', 'Company Fifteen', 'f47ac10b-58cc-4372-a567-0e02b2c3d486'),
  ('ba7c2119-8c71-49a0-8a3d-1d05ea91ea00', 'Company Sixteen', 'f47ac10b-58cc-4372-a567-0e02b2c3d486'),
  ('e1cb70e0-b4a5-44de-9e18-cbc0229e19ab', 'Company Seventeen', 'f47ac10b-58cc-4372-a567-0e02b2c3d487'),
  ('6f497e0e-cd7d-4b3d-bfa7-e77613b295b1', 'Company Eighteen', 'f47ac10b-58cc-4372-a567-0e02b2c3d487'),
  ('ff7c5820-92b1-48e3-bcd2-3520a6601b3c', 'Company Nineteen', 'f47ac10b-58cc-4372-a567-0e02b2c3d488'),
  ('139bc6b0-7849-497d-a9b4-97f56d1b0d56', 'Company Twenty', 'f47ac10b-58cc-4372-a567-0e02b2c3d488');

INSERT INTO [App].[Sales] ([Id], [Amount], [CreatedAt], [CompanyId], [TenantId])
VALUES
  ('98224b6e-f34b-4374-9257-8f48fa694e70', 1500.00, '2024-12-01', 'd02d41be-3e42-4f34-9d10-bd4d1a803433', 'f47ac10b-58cc-4372-a567-0e02b2c3d479'),
  ('46902c99-e23c-4967-9d34-156ef56c6f6d', 2200.00, '2024-12-02', 'd02d41be-3e42-4f34-9d10-bd4d1a803433', 'f47ac10b-58cc-4372-a567-0e02b2c3d479'),
  ('54822fd5-00b3-4b51-bb18-33bfe8e96a68', 1700.50, '2024-12-03', 'd02d41be-3e42-4f34-9d10-bd4d1a803433', 'f47ac10b-58cc-4372-a567-0e02b2c3d479'),
  ('23d4e75d-e157-45ed-b560-0c476c5733de', 2300.50, '2024-12-04', '8db1a120-b7a4-4e3f-b40f-3b1fc11fa6a1', 'f47ac10b-58cc-4372-a567-0e02b2c3d480'),
  ('d96b2795-d8f0-4d87-9ad7-0c2f7ec5e9d1', 1900.75, '2024-12-05', '8db1a120-b7a4-4e3f-b40f-3b1fc11fa6a1', 'f47ac10b-58cc-4372-a567-0e02b2c3d480'),
  ('6e41d2c9-e8ff-4f6d-89b6-ff9c5c0e2c3e', 2100.00, '2024-12-06', '8db1a120-b7a4-4e3f-b40f-3b1fc11fa6a1', 'f47ac10b-58cc-4372-a567-0e02b2c3d480');
```

5. Enter the Bearer Token in the Swagger UI and call the `GET /api/companies` endpoint.
6. The database query should return only two results instead of 20 (the total amount of companies in the database), as the EF Core global filter applies the `TenantId`.

---

## Migrations

### 1. Create Migration (Persistence Layer)

Run the following command to add a migration for the persistence layer (`AppDbContext`):

```bash
dotnet ef migrations add Init \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
    --context AppDbContext \
    --output-dir Persistence/Migrations
```

This will create a migration for the main persistence layer where your application's data resides.

### 2. Apply Migration (Persistence Layer)

Run the following command to apply the migration to the database:

```bash
dotnet ef database update \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
    --context AppDbContext
```

This will apply all pending migrations to the database.

### 3. Create Migration (Tenant Metadata)

Run the following command to add a migration for the tenant metadata (`TenantDbContext`):

```bash
dotnet ef migrations add AddTenantMetadata \
    --project src\SharedDatabase.Infrastructure \
    --startup-project src\SharedDatabase.Api \
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
    --startup-project src\SharedDatabase.Api \
    --context TenantDbContext
```

This will apply the tenant metadata migration to its respective database.

## Running the Project in Docker with HTTPS

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
