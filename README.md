# Soul Arenas API

## Database Migrations

### Prerequisites

- MySQL running (via docker-compose)
- .NET EF Core tools installed

**Install EF Core tools:**

```bash
dotnet tool install --global dotnet-ef
```

### Commands

**Create a new migration:**

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add MigrationName --project SoulArenasAPI/SoulArenasAPI.csproj --output-dir Database/Migrations
```

**Apply migrations to database:**

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project SoulArenasAPI/SoulArenasAPI.csproj
```

**Remove last migration:**

```bash
dotnet ef migrations remove --project SoulArenasAPI/SoulArenasAPI.csproj
```

### Quick Start

1. Start MySQL:

```bash
docker-compose up -d
```

2. Apply migrations:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project SoulArenasAPI/SoulArenasAPI.csproj
```

3. Run the application:

```bash
dotnet run --project SoulArenasAPI/SoulArenasAPI.csproj
```
