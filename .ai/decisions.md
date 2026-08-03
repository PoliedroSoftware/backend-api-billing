# Decisions

Architecture decisions recorded from the codebase. Baseline: 2026-08-02. Where a decision is inferred rather than explicitly documented, it is noted as inferred from the code.

## ADR-001 — Clean architecture with loose Domain boundaries
**Status:** Adopted (inferred from code structure)

Solution split into `Api` / `Application` / `Domain` / `Infraestructure.Persistence` / `Infraestructure.External.*`. However, provider-specific billing logic (Plemsi Prepare/Sender/Validate) lives inside `Poliedro.Billing.Domain\Common\Methods\Billing\`, not in the Application or Infraestructure layers — layer boundaries are deliberately loose.

## ADR-002 — Decouple external e-invoicing providers
**Status:** Adopted

Plemsi (main), Siigo, and TNS each get their own `Poliedro.Billing.Infraestructure.External.*` project with a hand-wired `AddExternal*` DI extension. Each exposes a domain service port (`ITNSDomainService`, `ISiigoDomainService`) and its own endpoint group. Recent commits (e.g. `a0d4ece decouple-resolutions-provider`) show active provider decoupling work.

## ADR-003 — Minimal APIs; MVC controllers compile-excluded
**Status:** Adopted

`Poliedro.Billing.Api` excludes `Controllers\**` from compilation (`Poliedro.Billing.Api.csproj`). All endpoints are static classes exposing `MapXEndpoints(this RouteGroupBuilder)` that must be manually wired in `Program.cs`. Route prefixes are inconsistent and mixed (e.g. `api/v1/billing`, `api/billing`, `api/v1/billing/sales/create`).

## ADR-004 — MediatR + domain `Result<TValue, TError>` pattern
**Status:** Adopted

Handlers are MediatR `IRequestHandler`s; commands/queries return the domain `Result<TValue, TError>` type. Endpoint handlers dispatch via `IMediator` and convert results to HTTP via `result.Match(...)` (`Poliedro.Billing.Api\Common\Extensions\ResultExtension.cs`), producing `ProblemDetails` on error.

## ADR-005 — Factory + strategy/selector for billing processing
**Status:** Adopted

`BillingPrepareFactory` (`IGetProcessorBilling`) and `BillingSenderFactory` (`IBillingSenderFactory`) resolve FE vs POS implementations keyed by `(ProviderType, ResolutionType)` from `IEnumerable<IBillingSender>` / `IServiceProvider`. Concrete strategies: `PrepareBillingFE`/`PrepareBillingPOS`, `BillingSenderFE`/`BillingSenderPOS`.

## ADR-006 — Specialized service ports composed into a domain service
**Status:** Adopted (inferred)

For CRUD features (Client, Server, DianResolution, CompanyProvider), small purpose-specific ports (`IClientExistsService`, `IClientCreateService`, ...) are registered alongside a composite `IClientDomainService` implementation that composes them (see `AddPersistence`).

## ADR-007 — AutoMapper profiles registered explicitly
**Status:** Adopted

No assembly scanning. Every profile must be added to the `MapperConfiguration` in `Poliedro.Billing.Application\DependencyInjectionService.cs`. `Billing\AutoMappers` folder is compile-excluded there; the actual billing profile lives under `Billing\AutoMappers\BillingAutoMapper.cs` and is registered by name.

## ADR-008 — MySQL via Pomelo with `ServerVersion.AutoDetect`
**Status:** Adopted

`AddDbContext` uses `UseMySql(connectionString, ServerVersion.AutoDetect(...))`. Connection chosen by `MYSQL_CONNECTION` env var, falling back to `ConnectionStrings:MysqlConnection`. Retail DB accessed through `DynamicDbContext` constructed with a caller-supplied per-company connection string.

## ADR-009 — Duplicated DB/MessageProvider registration accepted in `AddExternalPlemsi`
**Status:** Adopted (known trade-off)

`AddExternalPlemsi` registers its own `DataBaseContext` and `IMessageProvider`, duplicating `AddPersistence`. Decision: keep, because the provider project needs the context for billing persistence and the duplication is pre-existing.

## ADR-010 — Committed real credentials are the source of truth
**Status:** Adopted

`appsettings.json` contains live MySQL passwords and an SMTP app password and is committed. Deployments override via env vars (`MYSQL_CONNECTION`, `ConnectionStrings__*`). Do not remove/rotate committed credentials; do not add more.

## ADR-011 — Health checks depend on live remote MySQL
**Status:** Adopted

`/health` and `/health-ui` run `AspNetCore.HealthChecks.MySql` against the `MysqlConnection` string. App cannot start/respond healthy without network + DB access.

## ADR-012 — Provider-specific response/persistence handled per provider
**Status:** Adopted

TNS and Siigo use their own config models + settings services (`ApiTNS`, `ApiSiigo` sections) and REST clients. Siigo sends `Partner-ID: Test` header. Plemsi persistence uses `BillingResponseApi`, `DynamicDbContext` (retail), and `IUpdateCurrentlyNumber`.

## Non-decisions / open questions (not resolved in code)

- No authentication enforcement found on endpoints despite a Bearer JWT Swagger security scheme being documented.
- `ValidationBehaviour` registered twice; billing validator file empty — intended validation behavior is unclear.
- Worker service purpose/scope is undefined (loop commented out, not in solution).
