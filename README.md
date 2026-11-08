# MovieActorApi

A small ASP.NET Core and React project for learning C#, Entity Framework Core, PostgreSQL, CRUD, and a many-to-many relationship.

PostgreSQL must already be running on this machine. This project does not use Docker. The local connection string is only in `backend/appsettings.Development.json`:

`Host=localhost;Port=5432;Database=movieactor;Username=postgres;Password=postgres`

Create the `movieactor` database, then apply the migration.

## Backend

```bash
cd backend
dotnet tool install --global dotnet-ef
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update
dotnet run
```

The API listens on http://localhost:5080

## Frontend

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173. The page calls the API at http://localhost:5080.
