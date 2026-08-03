# Patterns

Recurring implementation patterns observed in the codebase. Baseline: 2026-08-02.

## Endpoint pattern (Minimal API)
Each feature has a static class with `MapXEndpoints(this RouteGroupBuilder)` that maps routes with `WithName`/`WithTags`/`WithSummary`/`Produces` metadata, and private static handler methods. Handlers take the command/query plus `IMediator`, send it, and return `result.Match(...)`.
- Reference: `Poliedro.Billing.Api\Endpoints\v1\Client\ClientEndpoints.cs`
- New endpoints must be **manually mapped in `Program.cs`** (route prefixes vary).

## MediatR command/query + domain Result
- Commands/queries are MediatR records (`IRequest<TResult>`) in `Poliedro.Billing.Application\<Feature>\Commands\<Name>\` / `Queries\<Name>\`.
- Handlers return the domain `Result<TValue, TError>` from `Poliedro.Billing.Domain\Common\Results\Result.cs` (implicit conversions + `Success`/`Failure`).
- HTTP conversion: `ResultExtension.Match` in `Poliedro.Billing.Api\Common\Extensions\ResultExtension.cs` maps `Error` → `ProblemDetails` (`ErrorResult`).

## FluentValidation pipeline
`ValidationBehaviour<TRequest,TResponse>` (Application\Common\Behaviors) runs all registered `IValidator<TRequest>` and throws `FluentValidation.ValidationException` on failure. Validators are auto-discovered by `FluentValidationConfiguration.AddFluentValidationServices()` (scans loaded assemblies). Registered as `IPipelineBehavior` in Application DI and again in `Program.cs`.

## Billing processing: factory + strategy/selector
- `BillingPrepareFactory` (`Application\Billing\Services\Factories\Plemsi\`) implements `IGetProcessorBilling`; switches on `(ProviderType, ResolutionType)` → `PrepareBillingFE` / `PrepareBillingPOS` (implement `ICreateBilling`).
- `BillingSenderFactory` (`External.Plemsi\Adapter\Billing\Impl\Plemsi\`) builds a dictionary keyed by `("PLEMSI","FE")`/`("PLEMSI","POS")` from `IEnumerable<IBillingSender>`; `Resolve` throws if missing.
- Request envelope: `PlemsiInvoiceRequest` (resolution + company provider + prepared invoices) in `Domain\Common\Methods\Billing\Sender\Plemsi\`.
- Post-send persistence: `BillingResponseApi` zips provider responses with invoices, resolves the per-company DB connection (`IServerGetByIdService` + `IDatabaseUtils`), inserts the invoice via `DynamicDbContext`, updates the resolution number.

## Wire DTOs via AutoMapper
Input DTOs (`CreateBillingInputDTO`) map to domain `CreateBilling`; domain entities map to snake_case Plemsi wire DTOs (`SenderRequestFEDTO` family in `Application\Billing\Dtos\Plemsi\FE\`, `InvoiceRequestPosDto` family in `...\POS\`). Profiles live under each feature's `AutoMappers\` folder and **must be registered by name** in `Application\DependencyInjectionService.cs`.

## CRUD domain services: specialized ports + composite
Features like Client, Server, DianResolution, CompanyProvider define a set of small ports (`I*ExistsService`, `I*CreateService`, `I*UpdateService`, `I*DeleteService`, `I*GetAllService`, `I*GetByIdService`) implemented per-feature under `Persistence.Mysql\<Feature>\DomainService\Impl\`, plus a composite `I*DomainService` implementation that composes them. All are registered scoped in `AddPersistence`.

## Persistence adapters
- `DataBaseContext` (billing DB): `DbSet`s for Server, DianResolution, CompanyProvider, ClientBillingElectronic, Invoices, ProviderEntities; entity configurations applied in `OnModelCreating` via `EntityConfiguration`.
- `DynamicDbContext` (retail DB): constructed per-tenant from a caller-supplied connection string; `DbSet<InvoiceEntity>` and `DbSet<InvoiceSuccessEntity>`.
- Both use `UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))`.

## Provider integration projects (Siigo / TNS)
Symmetrical structure per provider:
- `ConfigModels\AppSettings.cs` + `Services\ISettingsService.cs` / `Imp\SettingsService.cs` reading `Api<Provider>` config section (singleton).
- `DomainServices\<Provider>DomainService.cs` implementing the domain port, POSTing JSON with `System.Text.Json` + Bearer auth to `HostUrl + endpoint`.
- `DependencyInjectionService.cs` with `AddExternal<Provider>(config)` (scoped domain service, singleton settings).
- Siigo sends `Partner-ID: Test` header; TNS appends `codigosucursal` query param.

## Result/message resources
`IMessageProvider` returns Spanish validator message strings from `MessageProviderResource` (`.resx` + `.Designer.cs`, listed as solution items). Registered singleton in `Program.cs` and transient in both `AddPersistence` and `AddExternalPlemsi`.

## Domain enums / common models
- `Common\Enum\`: `DocumentType` (NIT=6, CC=3), `Automatic`, `MultipleResolution`.
- `Common\Pagination\`: `PaginationParams` (PageNumber=1, PageSize=10) + `PaginationResponse<T>`.
- `Common\Models\BaseResponseModel`: `StatusCode`, `Success`, `Message`, `dynamic Data`.
- Check digit: `CalculateCheckDigitsBilling` (weighted modulo-11, returns `"error"` on invalid input); `ValidateScriptBilling` classifies `-` → NIT else CC.

## Conventions to follow
- Never import or reference `Poliedro.Billing.Common` (empty project).
- Don't "fix" pinned package versions (NU1608) or the KubernetesClient vulnerability without asking.
- Don't chase the ~437 pre-existing build warnings.
- Don't remove/rotate committed credentials in `appsettings.json`; don't add more.
- Don't treat a green `dotnet test` as verification — tests are placeholders.
