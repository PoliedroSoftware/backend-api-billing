# AGENTS.md

## Build & Run

```powershell
# Restore + build (solution)
dotnet restore Poliedro.Billing.sln
dotnet build Poliedro.Billing.sln -c Release

# Run API locally
dotnet run --project Poliedro.Billing.Api\Poliedro.Billing.Api.csproj

# Run a single test project
dotnet test Poliedro.Billing.Api.Tests\Poliedro.Billing.Api.Tests.csproj
dotnet test Poliedro.Billing.Domain.Test\Poliedro.Billing.Domain.Test.csproj

# Health check (when running)
curl http://localhost:5062/health
```

## Architecture

Clean Architecture / Hexagonal on .NET:

```
Api (Minimal API) → Application (CQRS/MediatR) → Domain (models/ports)
                                                      ↓
Infraestructure.Persistence.Mysql  /  External.{Plemsi, Siigo, TNS}
```

- **API entrypoint:** `Poliedro.Billing.Api/Program.cs`
- **CQRS:** Commands/Queries live in `Application/`, handlers use MediatR + AutoMapper + FluentValidation
- **External integrations:** Plemsi (electronic invoicing), Siigo (accounting), TNS (billing)

## Important gotchas

### Mixed target frameworks
Most projects target `net10.0`, but `Poliedro.Billing.Common` and `WorkerServiceBilling` target `net8.0`. Both .NET 8 and .NET 10 SDKs are required.

### Projects NOT in the .sln
- `WorkerServiceBilling` — separate Worker Service, build/test it individually
- `Poliedro.Billing.Common` — also outside the solution

### Test projects are stubs
Both `Poliedro.Billing.Api.Tests` and `Poliedro.Billing.Domain.Test` contain only empty `[Fact]` placeholder tests. Tests do nothing currently.

### Hardcoded secrets (never commit changes to these)
- `appsettings.json` contains real DB connection strings and API credentials — treat as sensitive but already committed historically
- `WorkerServiceBilling/Program.cs` has a hardcoded connection string
- Email credentials in `EmailSettings`

### Docker
- No `docker-compose.yml`. Two separate Dockerfiles:
  - Root `Dockerfile` → builds the API (exposes 8080/8081)
  - `WorkerServiceBilling/Dockerfile` → builds the worker
- CI pushes to AWS ECR and deploys to ECS

### OpenAPI docs
Uses **Scalar** UI (not just Swagger). Launch profile opens `scalar/v1`. Swagger also available at `/swagger`.

### CI/CD (GitHub Actions)
Triggers on push to `main`/`feature/pipeline` and PR merge to `main`/`release/*`/`releasecandidate/*`. Pipeline: SonarCloud scan → build → test → JMeter load test → Docker build/push → AWS ECS deploy.

### Local Sonar analysis
```powershell
.\run-sonar.bat
```

### Worker service
The `Worker.cs` in `WorkerServiceBilling` has its execute logic entirely commented out — it does nothing at runtime.
