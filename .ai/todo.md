# Todo

Project memory. Baseline: 2026-08-02, branch `RefactorBilling` (up to date with `origin/RefactorBilling`), working tree clean.

## Known outstanding items

No formally tracked backlog exists in the repository (no issue tracker files, no TODO markers collected). The following are factual observations from the code, not confirmed work items:

- `Poliedro.Billing.Infraestructure.External.Plemsi\Adapter\Billing\Impl\Plemsi\GetLastInvoiceBillingPlemsi.cs` is a stub that always returns `1`; real last-invoice lookup logic is not implemented.
- `Poliedro.Billing.Application\Billing\Commands\CreateBilling\CreateBillingValidator.cs` is empty (0 lines) — the billing command has no FluentValidation rules.
- `Poliedro.Billing.Api\Common\Configurations\GlobalExceptionConfiguration.cs` — `OnException` body is empty; ProblemDetails helpers exist but are unreachable.
- `WorkerServiceBilling` — worker loop fully commented out and connection string hardcoded; not in the solution.
- Both test projects contain only empty placeholder tests and do not reference production projects.
- `.ai/` files should be kept in sync as meaningful milestones are reached.

## When adding entries

- One line per item, prefix with status when known (`[ ]` open, `[x]` done).
- Link back to the changelog entry or commit that closed an item.
