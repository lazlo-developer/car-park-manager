# Car Park Management API

Simple **.NET 9 Web API** built with **Clean Architecture**, using:
- PostgreSQL via Docker  
- Configuration through `.env` + .NET User Secrets  
- FluentValidation for input validation  
- Scalar for OpenAPI documentation  
- Shouldly + Moq for testing  

---

## Quick Start (PowerShell Only)

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/)

---
### 2. Clone the repository
```powershell
git clone https://github.com/yourname/carpark-management.git
cd carpark-management
````

--- 
### 3. Initialize environment and secrets

This script:

* Generates a random PostgreSQL password
* Creates/updates `.env`
* Stores the connection string in .NET User Secrets

```powershell
./init-db.ps1
```

---

### 4. Start PostgreSQL container

```powershell
docker compose up -d
```

---

### 5. Run the API

```powershell
dotnet run --project CarPark.Api
```

Docs available at:
 [http://localhost:5000/docs](http://localhost:5000/docs)

---

## Configuration

By default the API uses PostgreSQL, but you can also switch to an in-memory database  
by setting `"DatabaseProvider": "InMemory"` in `appsettings.json` or environment variables.

**appsettings.json**

```json
{
  "DatabaseProvider": "PostgreSQL",
  "ParkingRateSettings": {
    "SmallCarRatePerMinute": 0.10,
    "MediumCarRatePerMinute": 0.20,
    "LargeCarRatePerMinute": 0.40
  }
}
```

**.env** (auto-generated)

```
POSTGRES_USER=carpark_user
POSTGRES_DB=carparkdb
POSTGRES_PASSWORD=<generated>
POSTGRES_PORT=5432
```

---

## Tests

```powershell
dotnet test
```

---

## Summary

* Clean Architecture
* PostgreSQL + Docker
* Secure secrets via .NET User Secrets
* One-command local setup

```powershell
./init-db.ps1; docker compose up -d; dotnet run --project CarPark.Api
```

---

MIT © 2025 Jakub Lazar
