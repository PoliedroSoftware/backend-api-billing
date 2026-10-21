using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Api.Administration;

public static class BillingAdministrationEndpoints
{
    public static void MapBillingAdministration(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/admin").WithTags("Administración Odoo")
            .AddEndpointFilter<BillingAdminFilter>();
        group.MapGet("/status", (IOptions<BillingAdministrationOptions> options) => Results.Ok(new
        {
            service = "poliedro-billing", contractVersion = 1,
            licenseEnforcement = options.Value.IsConfigured,
            licenseAuthority = options.Value.LicenseUrl,
            checkedAt = DateTimeOffset.UtcNow
        }));
        group.MapGet("/clients", async (DataBaseContext db, CancellationToken ct) =>
            await db.ClientBillingElectronic.AsNoTracking().OrderBy(c => c.CompanyId)
                .Select(c => new { c.CompanyId, c.Name, c.Nit, c.Email, c.Active }).ToListAsync(ct));
        group.MapGet("/providers", async (DataBaseContext db, CancellationToken ct) =>
            await ProviderCatalog(db).ToListAsync(ct));
        group.MapGet("/servers", async (DataBaseContext db, CancellationToken ct) =>
            await db.Server.AsNoTracking().OrderBy(s => s.ServerId)
                .Select(s => new { s.ServerId, s.DatabaseName }).ToListAsync(ct));
        group.MapPost("/clients", UpsertClient);
        group.MapGet("/clients/{id:int}/resolutions", ClientResolutionStatus.Get);
        group.MapPost("/providers", UpsertProvider);
        group.MapGet("/automation/resolutions", async (DataBaseContext db, CancellationToken ct) =>
            await AutomationCandidates(db).ToListAsync(ct));
        group.MapGet("/automation/resolutions/{id:int}/eligibility", async (int id, DataBaseContext db,
            [FromServices] IBillingLicenseAuthorizer license, CancellationToken ct) =>
        {
            var candidate = await AutomationCandidates(db).SingleOrDefaultAsync(r => r.ResolutionId == id, ct);
            return candidate is null ? Results.Ok(new BillingLicenseDecision(false, "client_or_resolution_inactive"))
                : Results.Ok(await license.AuthorizeAsync(candidate.CompanyId, candidate.CompanyProviderId, ct));
        });
        group.MapPut("/clients/{id:int}/state", async (int id, ClientStateInput input, DataBaseContext db, CancellationToken ct) =>
        {
            var client = await db.ClientBillingElectronic.SingleOrDefaultAsync(c => c.CompanyId == id, ct);
            if (client is null) return Results.NotFound();
            client.Active = input.Active;
            await db.SaveChangesAsync(ct);
            return Results.Ok(new { client.CompanyId, client.Active });
        });
    }

    // Stable NIT identity and a serializable transaction make a lost-response retry safe.
    private static async Task<IResult> UpsertClient(ClientInput input, DataBaseContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Nit)
            || input.Nit.Length > 100 || input.Name.Length > 200 || (input.Email?.Length ?? 0) > 1000)
            return Results.BadRequest(new { message = "Nombre y NIT válidos son obligatorios." });
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var matches = await db.ClientBillingElectronic.Where(c => c.Nit == input.Nit.Trim()).ToListAsync(ct);
        if (matches.Count > 1 || (input.CompanyId > 0 && (matches.Count != 1 || matches[0].CompanyId != input.CompanyId)))
            return Results.Conflict(new { message = "La identidad del cliente requiere conciliación." });
        var entity = matches.SingleOrDefault();
        if (entity is null)
        {
            entity = new ClientEntity { Nit = input.Nit.Trim(), Active = false };
            db.ClientBillingElectronic.Add(entity);
        }
        entity.Name = input.Name.Trim();
        entity.Email = input.Email?.Trim() ?? "";
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Results.Ok(new { entity.CompanyId, entity.Name, entity.Nit, entity.Active });
    }

    private static async Task<IResult> UpsertProvider(ProviderInput input, DataBaseContext db, CancellationToken ct)
    {
        if (input.CompanyId <= 0 || input.ServiceId <= 0 || !Enum.IsDefined(input.EnvironmentType)
            || (input.ApiKey?.Length ?? 0) > 100)
            return Results.BadRequest(new { message = "Empresa, servidor o ambiente inválido." });
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        if (!await db.ClientBillingElectronic.AnyAsync(c => c.CompanyId == input.CompanyId, ct)
            || !await db.Server.AnyAsync(s => s.ServerId == input.ServiceId, ct))
            return Results.BadRequest(new { message = "La empresa y el servidor deben existir." });
        var legacy = await ProviderCatalog(db).Where(p => p.CompanyId == input.CompanyId && p.ServiceId == input.ServiceId).ToListAsync(ct);
        if (legacy.Any(p => p.Active is null || p.EnvironmentType is null))
            return Results.Conflict(new { message = "El proveedor existente tiene campos vacíos; requiere conciliación antes de modificarlo." });
        var matches = await db.CompanyProvider.Where(p => p.CompanyId == input.CompanyId
            && p.ProviderId == 1 && p.ServiceId == input.ServiceId).ToListAsync(ct);
        if (matches.Count > 1 || (input.CompanyProviderId > 0
            && (matches.Count != 1 || matches[0].CompanyProviderId != input.CompanyProviderId)))
            return Results.Conflict(new { message = "La identidad del proveedor requiere conciliación." });
        var entity = matches.SingleOrDefault();
        if (entity is null)
        {
            if (string.IsNullOrWhiteSpace(input.ApiKey))
                return Results.BadRequest(new { message = "La clave Plemsi es obligatoria para crear el proveedor." });
            entity = new CompanyProviderEntity { CompanyId = input.CompanyId, ProviderId = 1, ServiceId = input.ServiceId };
            db.CompanyProvider.Add(entity);
        }
        if (!string.IsNullOrWhiteSpace(input.ApiKey)) entity.ApiKey = input.ApiKey;
        entity.EnvironmentType = input.EnvironmentType;
        entity.Active = input.Active ? 1 : 0;
        var client = await db.ClientBillingElectronic.SingleAsync(c => c.CompanyId == input.CompanyId, ct);
        client.Active = input.Active;
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Results.Ok(new { entity.CompanyProviderId, entity.CompanyId, entity.Active });
    }

    public record ClientInput(int CompanyId, string Name, string Nit, string? Email);
    public record ClientStateInput(bool Active);
    public record ProviderInput(int CompanyProviderId, int CompanyId, int ServiceId,
        EnvironmentType EnvironmentType, string? ApiKey, bool Active);

    // Historical MySQL rows contain NULLs despite the old non-nullable EF entity.
    // Preserve those values in this read model; do not fabricate IDs or repair production data.
    private static IQueryable<ProviderCatalogEntry> ProviderCatalog(DataBaseContext db) =>
        db.Database.SqlQueryRaw<ProviderCatalogEntry>("""
            SELECT company_provider_id AS CompanyProviderId, company_id AS CompanyId,
                   server_id AS ServiceId, environment_type AS EnvironmentType, active AS Active
            FROM company_provider WHERE provider_id = 1
            """);

    private static IQueryable<AutomationResolution> AutomationCandidates(DataBaseContext db) =>
        db.Database.SqlQueryRaw<AutomationResolution>("""
            SELECT r.resolution_id AS ResolutionId, p.company_id AS CompanyId,
                   p.company_provider_id AS CompanyProviderId, c.name AS Name
            FROM dian_resolution_billing r
            JOIN company_provider p ON p.company_provider_id = r.company_provider_id
            JOIN billing_company c ON c.company_id = p.company_id
            WHERE r.active = 1 AND r.automatic = 1 AND p.active = 1 AND c.active = 1 AND p.provider_id = 1
              AND p.server_id IS NOT NULL AND r.document_type IN ('FE', 'POS')
              AND DATE(r.expiration_date) >= CURRENT_DATE
            """);

    public sealed class AutomationResolution
    {
        public int ResolutionId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyProviderId { get; set; }
        public string Name { get; set; } = "";
    }

    public sealed class ProviderCatalogEntry
    {
        public int CompanyProviderId { get; set; }
        public int? CompanyId { get; set; }
        public int? ServiceId { get; set; }
        public string? EnvironmentType { get; set; }
        public int? Active { get; set; }
    }
}
