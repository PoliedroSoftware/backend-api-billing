# Copilot Instructions for backend-api-billing

## Build, Test, and Development

### Build and Restore
```powershell
# Restore and build the entire solution
dotnet restore Poliedro.Billing.sln
dotnet build Poliedro.Billing.sln -c Release

# Restore and build a single project
dotnet restore Poliedro.Billing.Api\Poliedro.Billing.Api.csproj
dotnet build Poliedro.Billing.Api\Poliedro.Billing.Api.csproj -c Release
```

### Run Locally
```powershell
# Start the API (runs on http://localhost:5062)
dotnet run --project Poliedro.Billing.Api\Poliedro.Billing.Api.csproj

# Health check endpoint
curl http://localhost:5062/health

# Access API docs:
# - Scalar UI (default): http://localhost:5062/scalar/v1
# - Swagger UI: http://localhost:5062/swagger
```

### Testing
```powershell
# Run a single test project
dotnet test Poliedro.Billing.Api.Tests\Poliedro.Billing.Api.Tests.csproj
dotnet test Poliedro.Billing.Domain.Test\Poliedro.Billing.Domain.Test.csproj

# Run all tests
dotnet test Poliedro.Billing.sln
```

### Code Quality
```powershell
# Local SonarCloud analysis
.\run-sonar.bat
```

## Architecture

This is a **Clean Architecture** application with **Hexagonal/Ports-and-Adapters** pattern:

```
┌─────────────────────────────────────────────────────┐
│  API Layer (Minimal API)                            │
│  - Endpoints in Poliedro.Billing.Api/Endpoints/v1/  │
│  - Uses Scalar UI for OpenAPI docs                  │
└─────────────────────┬───────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────┐
│  Application Layer (CQRS + MediatR)                 │
│  - Commands: Poliedro.Billing.Application/*/Commands│
│  - Queries: Poliedro.Billing.Application/*/Queries  │
│  - Handlers use MediatR + AutoMapper + FluentVal.   │
│  - DTOs for request/response mapping                │
└─────────────────────┬───────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────┐
│  Domain Layer (Business Logic)                      │
│  - Poliedro.Billing.Domain/ (entities, value objs)  │
│  - Ports (interfaces) for infrastructure            │
└─────────────────────┬───────────────────────────────┘
                      │
         ┌────────────┼────────────┐
         │            │            │
┌────────▼──┐  ┌──────▼──┐  ┌─────▼──────┐
│ Persistence│  │ Plemsi  │  │Siigo │TNS  │
│ MySQL      │  │eInvoice │  │External    │
│Adapter     │  │Adapter  │  │Integrations│
└────────────┘  └─────────┘  └────────────┘
```

**Key Components:**
- **Poliedro.Billing.Api**: Minimal API endpoints, dependency injection setup
- **Poliedro.Billing.Application**: CQRS handlers, validators, AutoMapper profiles, DTOs, business services
- **Poliedro.Billing.Domain**: Core entities, value objects, domain services, port interfaces
- **Poliedro.Billing.Infraestructure.Persistence.Mysql**: EF Core DbContext, repository implementations
- **Poliedro.Billing.Infraestructure.External.{Plemsi,Siigo,TNS}**: Integration adapters for external services

## Important Architecture Notes

### CQRS Pattern Usage
- **Commands** (write operations): located in `Application/{Feature}/Commands/{CommandName}/`
  - Typically named: `{Action}{Entity}Command`, `{Action}{Entity}CommandHandler`, `{Action}{Entity}CommandValidator`
- **Queries** (read operations): located in `Application/{Feature}/Queries/{QueryName}/`
  - Typically named: `Get{Entity}{Criteria}Query`, `Get{Entity}{Criteria}QueryHandler`
- All handlers extend MediatR handlers and use AutoMapper for DTO mapping

### Validation
- Use **FluentValidation** for all command/query validators
- Validators inherit from `AbstractValidator<TRequest>`
- Validators are auto-registered via dependency injection in `Program.cs`

### Mapping
- Use **AutoMapper** profiles for DTO ↔ Domain entity mapping
- Profiles are located in `Application/{Feature}/AutoMappers/` (e.g., `ClientMapper.cs`)
- Register profiles in `DependencyInjectionService.cs` in the Application layer

### External Integrations
- **Plemsi**: Electronic invoicing (FE) and Point-of-Sale (POS) billing
  - Adapters handle FE vs POS invoice preparation via `PrepareBillingFE` and `PrepareBillingPOS`
  - Uses factory pattern: `BillingPrepareFactory`
- **Siigo**: Accounting system integration
- **TNS**: Billing system integration

## Key Conventions

### Namespace Structure
- Follow Clean Architecture layers: `Poliedro.Billing.{Layer}`
- Features organized by domain aggregate: `.../{Feature}/Commands`, `.../{Feature}/Queries`, `.../{Feature}/Dtos`, etc.

### Endpoint Routing
- Endpoints located in `Poliedro.Billing.Api/Endpoints/v1/`
- One endpoint class per resource/operation (e.g., `GetClientBillingElectronicByIdEndpoint.cs`)
- Endpoints map HTTP requests to MediatR commands/queries
- API version in URL: `/api/v1/...`

### Error Handling
- Domain errors use builder pattern: `{Entity}BillingElectronicErrorBuilder.cs` (e.g., `ClientBillingElectronicErrorBuilder.cs`)
- Errors are structured and mapped in Application layer before returning to API

### Database Access
- Use Entity Framework Core with MySQL
- DbContext in `Poliedro.Billing.Infraestructure.Persistence.Mysql`
- Migrations managed via EF Core Tools

### Dependencies Injection
- Main setup: `Poliedro.Billing.Api/Program.cs` (registers all layers, external services, health checks)
- Application layer setup: `Poliedro.Billing.Application/DependencyInjectionService.cs` (registers MediatR, AutoMapper, validators, handlers)
- External services: Each external adapter (Plemsi, Siigo, TNS) has its own registration in Program.cs

### Nullable Reference Types
- Enabled across all projects (`<Nullable>enable</Nullable>` in .csproj)
- Always handle null checks and use `#nullable enable/disable` pragmas where needed

### Logging
- Use `ILogger<T>` injected via dependency injection
- Log level set to Debug by default locally

## Important Gotchas

### Mixed Target Frameworks
- Most projects target `.net10.0` (including Poliedro.Billing.Api, Application, Domain, Infrastructure)
- **EXCEPTION**: `Poliedro.Billing.Common` and `WorkerServiceBilling` target `.net8.0`
- Ensure both .NET 8 and .NET 10 SDKs are installed

### Projects Outside the Solution
- **WorkerServiceBilling**: Separate worker service, build/test individually with `dotnet` commands
- **Poliedro.Billing.Common**: Also outside the main solution

### Hardcoded Secrets (DO NOT MODIFY)
- `appsettings.json` contains real DB connection strings and API credentials
- Treat as sensitive data but historically committed to the repo
- `WorkerServiceBilling/Program.cs` has hardcoded connection string
- `EmailSettings` contains email credentials
- **NEVER commit credential changes**

### Test Projects Are Currently Stubs
- Both `Poliedro.Billing.Api.Tests` and `Poliedro.Billing.Domain.Test` contain only empty `[Fact]` placeholder tests
- Tests do not execute meaningful assertions currently

### Docker & CI/CD
- No `docker-compose.yml` file
- Two separate Dockerfiles:
  - Root `Dockerfile`: Builds the API (exposes 8080/8081)
  - `WorkerServiceBilling/Dockerfile`: Builds the worker service
- GitHub Actions CI/CD:
  - Triggers on: push to `main`/`feature/pipeline`, PR merge to `main`/`release/*`/`releasecandidate/*`
  - Pipeline: SonarCloud scan → build → test → JMeter load test → Docker build/push → AWS ECS deploy
- Images pushed to AWS ECR; deployment to AWS ECS

### Worker Service Note
- `WorkerServiceBilling/Worker.cs` has all execute logic commented out — does nothing at runtime

## Common Tasks

### Adding a New Endpoint
1. Create feature folder under `Poliedro.Billing.Application/{FeatureName}/`
2. Add Command/Query in `Commands/` or `Queries/` subfolder
3. Add corresponding CommandHandler/QueryHandler
4. Add CommandValidator/QueryValidator
5. Create DTO classes if needed
6. Add AutoMapper profile in `AutoMappers/` subfolder
7. Create endpoint class in `Poliedro.Billing.Api/Endpoints/v1/{FeatureName}/`
8. Register endpoint in `Program.cs` via minimal API builder (`app.MapGet`, `app.MapPost`, etc.)
9. Test with `dotnet run` and access via Swagger/Scalar UI

### Adding External Service Integration
1. Create adapter in `Poliedro.Billing.Infraestructure.External.{ServiceName}/`
2. Define port (interface) in `Poliedro.Billing.Domain/Ports/`
3. Implement adapter for the port
4. Register in `Program.cs` with `AddScoped<IPort, Adapter>()`
5. Inject into command/query handlers via constructor

### Running Database Migrations
```powershell
# Add migration
dotnet ef migrations add {MigrationName} --project Poliedro.Billing.Infraestructure.Persistence.Mysql

# Update database
dotnet ef database update --project Poliedro.Billing.Infraestructure.Persistence.Mysql
```
