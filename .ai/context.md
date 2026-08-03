# Project Context

Persistent project memory. Baseline captured on 2026-08-02 from a clean checkout of branch `RefactorBilling`.

## What this is

`.NET 10` clean-architecture billing API for **Poliedro** implementing Colombian electronic invoicing (facturación electrónica / DIAN). Monorepo of one solution (`Poliedro.Billing.sln`) plus a standalone worker project not in the solution.

- GitHub: `https://github.com/PoliedroSoftware/backend-api-billing.git`
- Main provider integration is **Plemsi**; **Siigo** and **TNS** are separate endpoint groups.
- Two MySQL databases: billing DB (`billing`) and retail DB (`eduar_retail`).

## Tech stack

- .NET 10 (net10.0), C#, no `global.json`
- ASP.NET Core **Minimal APIs** (MVC controllers compile-excluded)
- MediatR (commands/queries + `ValidationBehaviour` pipeline)
- AutoMapper (profiles registered explicitly)
- FluentValidation (validators auto-scanned; `ValidationBehaviour` in pipeline)
- EF Core 9 over MySQL via `Pomelo.EntityFrameworkCore.MySql` 9.0.0 (pinned; NU1608 warning expected), `ServerVersion.AutoDetect`
- Swashbuckle + Scalar.AspNetCore for OpenAPI
- xUnit 2.5.3 (test projects are placeholder-only)
- AWS ECR/ECS + on-premise Docker deployment via GitHub Actions

## Commands

- Build: `dotnet build` (solution root). Succeeds with ~437 pre-existing nullable/other warnings — do not chase them.
- Test: `dotnet test`. **Both test projects contain only empty `UnitTest1` placeholders — green tests prove nothing.**
- Run API: `dotnet run --project Poliedro.Billing.Api` → http://localhost:5062; docs at `http://localhost:5062/scalar/v1`; Swagger JSON at `/swagger/v1/swagger.json`.
- Health: `/health` (UI at `/health-ui`) — both hit a live remote MySQL; startup/health need network + DB.
- Local Sonar scan: `run-sonar.bat` (contains a SonarCloud token).

## Solution layout

| Project | Role |
|---|---|
| `Poliedro.Billing.Api` | Minimal-API host. `Controllers\` folder compile-excluded. Swagger/Scalar, CORS, health checks, endpoint mapping. |
| `Poliedro.Billing.Domain` | Entities, ports, domain services. Also hosts provider-specific billing logic under `Common\Methods\Billing\Prepare\|Sender\|Validate\Plemsi` (loose layer boundaries). |
| `Poliedro.Billing.Application` | MediatR commands/queries + AutoMapper profiles. `Billing\AutoMappers` folder is compile-excluded there. |
| `Poliedro.Billing.Infraestructure.Persistence.Mysql` | EF Core over Pomelo MySQL. `DataBaseContext` = billing DB; `DynamicDbContext` = per-tenant retail DB. |
| `Poliedro.Billing.Infraestructure.External.Plemsi` | Plemsi provider adapters (FE/POS billing, senders, response persistence, email, location). Registers its own `DataBaseContext` + `IMessageProvider`. |
| `Poliedro.Billing.Infraestructure.External.Siigo` | Siigo invoicing + DIAN submission (`ISiigoDomainService`). |
| `Poliedro.Billing.Infraestructure.External.TNS` | TNS sales creation (`ITNSDomainService`). |
| `Poliedro.Billing.Common` | **Empty project (csproj only). Do not reference it.** |
| `Poliedro.Billing.Api.Tests` / `Poliedro.Billing.Domain.Test` | Placeholder-only test projects (xUnit, no project references, empty `[Fact]`). |
| `WorkerServiceBilling` | Separate worker, **not in the solution**. net8.0; worker loop fully commented out; hardcoded MySQL connection string. |

## Endpoint groups (mapped in `Program.cs`)

Route prefixes vary and are mixed — always check `Program.cs` before adding a route:

- `api/v1/billing` → `MapBillingEndpoints`
- `api/v1/client` → `MapClientEndpoints`
- `api/v1/dianresolution` → `MapDianResolutionEndpoints`
- `api/v1/Controllers/v1/FERetail` → `MapFERetailEndpoints`
- `api/v1/getinvoice` → `MapGetInvoiceEndpoints`
- `api/v1/invoicespendingwithdetails` → `MapInvoicesPendingWithDetailsEndpoints`
- `api/billing` → `MapPdfInvoiceEndpoints`
- `api/v1/server` → `MapServerEndpoints`
- `api/v1/billing/invoices` → `MapSiigoEndpoints`
- `api/v1/invoice` → `MapSuccessInvoiceEndpoints`
- `api/v1/billing/sales/create` → `MapTnsEndpoints`
- `api/v1/customers` → `MapCustomersIdEndpoints`
- `api/v1/location` → `MapLocationEndpoints`
- `api/v1/companyProvider` → `MapCompanyProviderEndpoints`

## Core billing flow (Plemsi)

`CreateBillingHandler` → `BillingPrepareFactory` selects `PrepareBillingFE`/`PrepareBillingPOS` by `(ProviderType, ResolutionType)` → prepared wire DTOs wrapped in `PlemsiInvoiceRequest` → `BillingSenderFactory.Resolve` → `BillingSenderFE`/`BillingSenderPOS` POST to Plemsi (Bearer auth, number guard) → on success `BillingResponseApi` persists invoice + updates the resolution's current number.

## Configuration (`appsettings.json`)

Real credentials are committed and are **the source of truth** — do not remove/rotate them, do not add more. Deployments override via env vars `MYSQL_CONNECTION`, `ConnectionStrings__MysqlConnection`, `ConnectionStrings__MysqlConnectionRetail`.

- `ConnectionStrings`: `MysqlConnection` (billing DB), `MysqlConnectionRetail` (retail DB)
- `ApiPlemsi` / `ApiPlemsiQa`: Plemsi URLs; `Enviroment:Production` (sic) selects production vs QA URLs
- `ApiTNS`: `hostUrl`, `createSaleEndPoint`, `mediaType`
- `ApiSiigo`: `hostUrl`, `createInvoceAndSendDianEndPoint`, `mediaType`
- `EmailSettings`: SMTP (Gmail app password), company email(s), subject
- `AllowedOrigins`: CORS policy `PoliedroBilling` origins (includes `*`)
- `CosumerFinal`: fallback identification/dv used when check-digit computation fails
- `Logging`, `AllowedHosts`

## DI registration (hand-wired — nothing auto-discovered)

`Program.cs` chain: `AddWebApi().AddApplication().AddExternalPlemsi(config).AddExternalTns(config).AddExternalSiigo(config).AddPersistence(config)`.

- Each project's `DependencyInjectionService.cs` exposes a static `Add*` extension wired by hand in `Program.cs`.
- New AutoMapper profiles must be added explicitly in `MapperConfiguration` in `Poliedro.Billing.Application\DependencyInjectionService.cs`.
- MediatR handlers auto-register via `RegisterServicesFromAssembly`.
- `AddExternalPlemsi` registers its own `DataBaseContext` and `IMessageProvider`, duplicating what `AddPersistence` registers; both use the `MYSQL_CONNECTION` env var (or `MysqlConnection` string).
- `ValidationBehaviour` is registered twice (Application DI + `Program.cs` line 110).

## Deployment / CI

- `.github/workflows/main.yml` — push to `main`/`feature/pipeline` or merged PR into `main`/`release/*`/`releasecandidate/*`: SonarCloud + build + test + JMeter (`LoadTest/Billing.jmx`) → Docker → ECR → ECS (`update-service --force-new-deployment`).
- `.github/workflows/deploy-onpremise.yml` — push to `releasecandidate/v1.0.0`: Docker image shipped over Cloudflare-tunneled SSH to an on-prem host, run on ports 8080/8081 with env overrides.
- `Dockerfile` — multi-stage net10.0 publish of `Poliedro.Billing.Api`, exposes 8080/8081, `USER app`.

## Known quirks

- `Pomelo.EntityFrameworkCore.MySql` pinned at 9.0.0 against EF Core 10 → NU1608 warning expected. `KubernetesClient 15.0.1` has a known moderate vulnerability. Both pre-existing; don't "fix" without asking.
- `ServerVersion.AutoDetect` against live remote MySQL — an agent without network/DB access cannot run the app end-to-end.
- No `global.json`; requires .NET 10 SDK.
- `GetLastInvoiceBillingPlemsi` is a stub returning `1`.
- `CreateBillingValidator.cs` and the global exception filter are effectively empty (validation/error handling effectively not active).
