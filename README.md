# Workshop API

This repository hosts my ASP.NET Core Web API assignment where I build a CRUD API for managing workshops with DTOs, services, and Entity Framework Core. The goal is to practice structuring a real project, keeping controllers thin, and wiring up async data access via EF Core. I'm opting to run SQL Server in Docker instead of LocalDB so I can get comfortable with containerized databases.

## Local SQL Server via Docker

1. Copy the env template and set a strong password:
   ```bash
   cp .env.example .env
   # edit .env to set SA_PASSWORD=<your strong password>
   ```
2. Start the container (Docker Desktop/daemon must be running):
   ```bash
   docker compose up -d workshop-sql
   ```
3. Confirm the database is ready:
   ```bash
   docker ps --filter name=workshop-sql
   docker logs -f workshop-sql   # look for "SQL Server is now ready"
   ```
4. Stop it when you are done:
   ```bash
   docker compose down
   ```

The compose file creates a named volume (`workshop-sql-data`) so data persists across restarts.

## Connection String

Point `appsettings.Development.json` (or user secrets) at the container:

```json
"ConnectionStrings": {
  "Default": "Server=localhost,1433;Database=WorkshopDb;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=False;TrustServerCertificate=True"
}
```

Replace the password with the value set in `.env`. With that in place, `dotnet ef database update` will target the Docker-hosted SQL Server instance.
