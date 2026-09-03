# Changelog

Change log maintained as project memory. Baseline: 2026-08-02 from `git log` (branch `RefactorBilling`). Format follows the repo's mixed Spanish/English, conventional-commit style.

## Recent history (newest first)

| Date | Commit | Message |
|---|---|---|
| 2026-08-01 | `1e321dc` | fix(route billing) |
| 2026-08-01 | `24a59ae` | refactor |
| 2026-07-16 | `691a684` | feat: agrega CRUD completo para CompanyProvider (Create, Update, Delete) |
| 2026-07-02 | `77cb0de` | Resolve merge conflict |
| 2026-07-02 | `b98a641` | Merge fix/company-provider-endpoint into merge-fucion |
| 2026-07-02 | `f0fad32` | Merge fixyj into merge-fucion |
| 2026-07-02 | `d2e6cb0` | fix(dian-resolution): corregir conversión de enum ResolutionType desde base de datos |
| 2026-06-30 | `f708e67` | fix: corrige binding de PageNumber requerido en GET /api/v1/server |
| 2026-06-28 | `5f09401` | fix: company provider endpoint - fix column mappings and automapper configuration |
| 2026-06-28 | `69b6893` | problemas de logica |
| 2026-06-25 | `814883d` | fix: company provider endpoint - fix column mappings and automapper configuration |
| 2026-06-19 | `fbb24d7` | fix |
| 2026-06-18 | `57dad9e` | fix |
| 2026-06-18 | `3c725b8` | fix |
| 2026-06-17 | `5c10401` | fix(refactor) |
| 2026-06-12 | `d64a7dc` | fix(Refactor) |
| 2026-05-25 | `45650f6` | refactorCode |
| 2026-05-20 | `2fa5bd3` | Merge branch 'releasecandidate/v1.0.0' into DecoupleProviders |
| 2026-05-14 | `a0d4ece` | decouple-resolutions-provider |
| 2026-05-14 | `ff453af` | fix: health check via SSH tunnel + container logs for debugging |
| 2026-05-14 | `a5ae8d8` | trigger: restart container |
| 2026-05-14 | `eb85d4c` | trigger: deploy with new SSH hostname |
| 2026-05-14 | `081f7b5` | fix: health check response writer and connection string env var |
| 2026-05-14 | `4d88cd4` | feat: use cloudflared for on-premise SSH deploy |
| 2026-05-12 | `0a742a1` | trigger: test on-premise deploy via cloudflare tunnel |

## Recurring themes in history

- **CompanyProvider CRUD** — built up over several commits (2026-06 to 07) with repeated fixes to column mappings and AutoMapper configuration; completed in `691a684`.
- **Provider decoupling** — `decouple-resolutions-provider`, merges of `feature/...`/`fix/...` branches into `merge-fucion` and `DecoupleProviders`; PRs merged for `feature/deploy-on-premise`.
- **On-premise deployment** — Cloudflare-tunneled SSH deploy (cloudflared `access tcp`), container restart triggers, health-check debugging through SSH tunnel (May 2026).
- **Bug fixes** — DianResolution enum conversion from DB (`d2e6cb0`), required PageNumber binding in `GET /api/v1/server` (`f708e67`).

## Pre-existing state at baseline (2026-08-02)

- 231 commits total; no git tags present.
- Branch tips: `main` and `releasecandidate/v1.0.0` exist on `origin`; many stale local/remote feature branches remain (e.g. `creditnote`, `BillingPOS`, `FixHandlerBilling`, `feat/standardize-invoices-response`).
- Both test projects contain only empty `UnitTest1` placeholders.
- `Poliedro.Billing.Common` is an empty project; `WorkerServiceBilling` is not in the solution.

## Maintenance convention

Append new entries at the top of "Recent history" when milestones are reached; group related work under "Recurring themes" only when a theme takes shape. Never fabricate dates or hashes — pull them from `git log` / `git status` at the time of writing.
