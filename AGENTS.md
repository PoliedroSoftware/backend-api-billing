# AGENTS.md

.NET 10 clean-architecture billing API (Colombian electronic invoicing) for Poliedro. Monorepo of one solution plus a standalone worker.

## Commands

- Build: `dotnet build` (solution root). Succeeds with ~437 pre-existing nullable/other warnings — do not chase them.
- Test: `dotnet test`. **Both test projects (`Poliedro.Billing.Api.Tests`, `Poliedro.Billing.Domain.Test`) contain only empty `UnitTest1` placeholders** — tests pass trivially and verify nothing. Never treat a green `dotnet test` as proof a change works.
- Run API: `dotnet run --project Poliedro.Billing.Api` → http://localhost:5062, docs at `http://localhost:5062/scalar/v1` (Swagger JSON: `/swagger/v1/swagger.json`).
- Health: `/health`, UI at `/health-ui`. Both hit a live remote MySQL, so startup/health needs network + DB.
- No `global.json`; requires .NET 10 SDK. Local Sonar scan: `run-sonar.bat` (contains a SonarCloud token).

## Architecture

- `Poliedro.Billing.Api` — minimal-API host. **The `Controllers\` folder is excluded from compilation in the csproj**; this is endpoint-based, not MVC.
- `Poliedro.Billing.Domain` — entities, ports, domain services. Also holds provider-specific billing logic under `Common\Methods\Billing\Prepare|Sender|Validate\Plemsi` (layer boundaries are loose; application-ish logic lives in Domain).
- `Poliedro.Billing.Application` — MediatR commands/queries + AutoMapper profiles.
- `Poliedro.Billing.Infraestructure.Persistence.Mysql` — EF Core over Pomelo MySQL. `DataBaseContext` = billing DB; `DynamicDbContext` = retail DB.
- `Poliedro.Billing.Infraestructure.External.{Plemsi,Siigo,TNS}` — external e-invoicing providers. Billing flow is Plemsi; Siigo/TNS are separate endpoint groups.
- `Poliedro.Billing.Common` — **empty project (csproj only, no code). Don't reference it.**
- `WorkerServiceBilling` — separate worker, **not in the solution file**. Its worker loop is fully commented out and it has a hardcoded MySQL connection string.

## DI registration gotchas

- Each project's `DependencyInjectionService.cs` exposes a static `Add*` extension wired by hand in `Program.cs`. Nothing is auto-discovered.
- **New AutoMapper profiles must be added explicitly** in the `MapperConfiguration` in `Poliedro.Billing.Application\DependencyInjectionService.cs` (also note `Billing\AutoMappers` is compile-excluded there).
- MediatR handlers are found via `RegisterServicesFromAssembly` — those auto-register.
- `AddExternalPlemsi` registers its own `DataBaseContext` and `IMessageProvider`, duplicating what `AddPersistence` registers. `MYSQL_CONNECTION` env var (or `MysqlConnection` connection string) decides the DB for both.

## Config

- Real credentials live in `Poliedro.Billing.Api\appsettings.json` (MySQL passwords, SMTP app password) and are committed — do not remove/rotate them, do not add more. Deployments override via `MYSQL_CONNECTION`, `ConnectionStrings__MysqlConnection`, `ConnectionStrings__MysqlConnectionRetail`.
- Sections: `ApiPlemsi` / `ApiPlemsiQa`, `ApiTNS`, `ApiSiigo`, `EmailSettings`, `AllowedOrigins`.

## Conventions

- New endpoints: static class with `Map<X>Endpoints(this RouteGroupBuilder)` (see `Endpoints\v1\Client\ClientEndpoints.cs`), then **manually map it in `Program.cs`**. Route prefixes vary and are mixed (e.g. `api/v1/billing`, `api/billing`, `api/v1/billing/sales/create`) — check Program.cs before adding a route.
- Handlers return domain results matched via `result.Match(onSuccess => ...)`; endpoint handlers use `IMediator`.
- Commit messages are mixed Spanish/English, conventional-commit style. Feature/bug branches use `feature/...`, `fix/...`.

## CI / deploy

- `.github/workflows/main.yml` — on push to `main`/`feature/pipeline` or PR merge into main/`release/*`/`releasecandidate/*`: SonarCloud + build + test + JMeter (`LoadTest/Billing.jmx`), then Docker → ECR → ECS.
- `.github/workflows/deploy-onpremise.yml` — push to `releasecandidate/v1.0.0`: Docker image shipped over a Cloudflare-tunneled SSH to an on-prem host, run on ports 8080/8081.

## Known quirks

- `Pomelo.EntityFrameworkCore.MySql` is pinned at 9.0.0 against EF Core 10 → NU1608 warning is expected. `KubernetesClient 15.0.1` has a known moderate vulnerability. Both are pre-existing; don't "fix" them without asking.
- App uses `ServerVersion.AutoDetect` against live remote MySQL — an agent without network/DB access cannot run the app end-to-end.


# AI Workflow

You are the long-term maintainer of this repository.

Your responsibility is to preserve project knowledge between sessions.

The `.ai/` directory is the project's persistent memory.

## Session Start

Before writing code:

- Read every file inside `.ai/`.
- Build an understanding of the current work, pending tasks, and previous decisions.

## During Development

Keep the project memory synchronized with the codebase.

Only update memory when a meaningful milestone is reached, such as:

- Feature completed
- Bug fixed
- Refactor completed
- Architecture changed
- New dependency introduced
- New coding pattern adopted

Do not rewrite memory files unnecessarily.

## Session End

If project knowledge has changed:

- Update `.ai/context.md`
- Update `.ai/todo.md`
- Update `.ai/decisions.md`
- Update `.ai/changelog.md`
- Update `.ai/patterns.md` only when a reusable pattern emerges.

## Memory Rules

- Documentation must describe reality, never intentions.
- Keep documents concise.
- Remove obsolete information.
- Never invent facts.
- Prefer updating existing files over creating new ones.
- Avoid duplicate information across memory files.



## Requirement Clarification

If the request is ambiguous, incomplete, or could have multiple valid implementations:

- Ask clarifying questions first.
- Do not assume business rules.
- Do not infer missing requirements.
- Help the user refine the idea before proposing a solution.

## Planning First

Before implementing any non-trivial change:

1. Analyze the existing implementation.
2. Explain your understanding of the current flow.
3. Identify assumptions and uncertainties.
4. Propose one or more implementation strategies.
5. Recommend the best option and explain why.
6. Present a step-by-step implementation plan.
7. Wait for explicit approval before making any file changes.

Do not create, modify, rename, or delete files until the implementation plan has been approved.


For small and obvious tasks (renaming, formatting, fixing a typo), implementation can proceed immediately.