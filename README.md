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

Copy `appsettings.Development.example.json` to `appsettings.Development.json` (or use user secrets) and point the connection string at the container:

```json
"ConnectionStrings": {
  "Default": "Server=localhost,1433;Database=WorkshopDb;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=False;TrustServerCertificate=True"
}
```

Replace the password with the value set in `.env`. With that in place, `dotnet ef database update` will target the Docker-hosted SQL Server instance.

## JWT Demo Settings

`appsettings.Development.example.json` ships with a `Jwt` section containing `Issuer`, `Audience`, `Key`, and `DemoPassword`. The `/api/auth/token` endpoint expects the configured `DemoPassword` and issues a short-lived JWT signed with the configured key. Update those values before deploying anywhere beyond local development.

## Apply EF Core Migrations

1. Ensure Docker SQL Server is running (see above).
2. Restore dependencies and build:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Apply the initial migration from the API project directory:
   ```bash
   dotnet ef database update --project src/Workshop.Api/Workshop.Api.csproj
   ```
   The command creates the `Workshops` and `Sessions` tables in the Docker database.

## Run the API

From `src/Workshop.Api` start the application:

```bash
dotnet run
```

The default profile hosts the API at `https://localhost:7230` and `http://localhost:5230` (check `Properties/launchSettings.json`).

## Sample Requests

`src/Workshop.Api/Workshop.Api.http` contains ready-to-run examples for all endpoints:

- `POST /api/auth/token` — exchange the `Jwt:DemoPassword` for a JWT (copy the returned token into the `@jwtToken` variable).
- `GET /api/workshops?search=` — list or search workshops.
- `GET /api/workshops/{workshopId}` — fetch a single workshop.
- `POST /api/workshops` — create a workshop (body includes `title`, `description`, `date`, `maxParticipants`).
- `PUT /api/workshops/{workshopId}` / `DELETE /api/workshops/{workshopId}` — update or remove workshops (DELETE requires `Authorization: Bearer <token>`).
- Nested session endpoints under `/api/workshops/{workshopId}/sessions` cover GET/POST/PUT/DELETE (DELETE also requires the bearer token).

Use the `.http` file via VS Code’s REST Client extension or import the requests into Postman. Update the `@workshopId`, `@sessionId`, `@date`, and `@jwtToken` variables with real values returned from previous calls.
