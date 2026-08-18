# Todo

Project memory. Baseline: 2026-08-02, branch `RefactorBilling` (up to date with `origin/RefactorBilling`), working tree clean.

## Known outstanding items

Closed during the billing endpoint hardening (2026-08-17, branch `RefactorBilling`): `CreateBillingValidator` implemented (validation active); global exception handling active via `GlobalExceptionHandler` (`IExceptionHandler` + ProblemDetails, `UseExceptionHandler`); empty `GlobalExceptionConfiguration` filter removed; `CreateBillingInputDTO` merged into `CreateBillingDTO`; `IHttpClientFactory` applied to Plemsi senders/last-number repos; `BillingResponseApi` null-safety (Server/Data/Numeration); senders' cast + config null-safety; `ILogger` in billing handler; `CreateBillingCommandResult` (404/400 + per-invoice results).

Remaining (no code fixes pending in the priority list; some are documentation-only):
- [ ] `WorkerServiceBilling` — worker loop fully commented out, hardcoded connection string, not in the solution. Dead code; not worth touching (group D).
- [ ] Both test projects contain only empty placeholder tests and do not reference production projects.
- [ ] Authentication not enforced on endpoints despite the documented Bearer JWT scheme — to be evaluated separately (group C, item 15).
- [ ] `ValidationBehaviour` registered twice (Application DI + `Program.cs` line 108).
- [ ] 7 remaining `new HttpClient()` outside the Plemsi billing flow (Siigo/TNS/PdfInvoice/InvoicePos/SendMessage/GetInvoice/CustomersId) — candidates for `IHttpClientFactory`, out of scope.

## When adding entries

- One line per item, prefix with status when known (`[ ]` open, `[x]` done).
- Link back to the changelog entry or commit that closed an item.
