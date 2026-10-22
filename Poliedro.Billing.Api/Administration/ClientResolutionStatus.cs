using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Api.Administration;

public static class ClientResolutionStatus
{
    public static async Task<IResult> Get(int id, DataBaseContext db,
        [FromServices] IHttpClientFactory clients, IConfiguration configuration, CancellationToken ct)
    {
        if (!await db.ClientBillingElectronic.AsNoTracking().AnyAsync(c => c.CompanyId == id, ct))
            return Results.NotFound();
        var rows = await db.Database.SqlQuery<ResolutionStatus>($"""
            SELECT r.resolution_id AS ResolutionId, r.company_provider_id AS CompanyProviderId,
                   r.document_type AS DocumentType, r.resolution_number AS ResolutionNumber,
                   r.prefix AS Prefix, r.initial_range AS InitialRange, r.final_range AS FinalRange,
                   r.current_number AS CurrentNumber, r.resolution_date AS ResolutionDate,
                   r.expiration_date AS ExpirationDate, r.date_current AS BillingUpdatedAt
            FROM dian_resolution_billing r
            JOIN company_provider p ON p.company_provider_id = r.company_provider_id
            WHERE p.company_id = {id} AND p.provider_id = 1 AND r.active = 1
              AND r.document_type IN ('FE', 'POS')
            """).ToListAsync(ct);
        // Credentials are read separately and never become response properties.
        var keys = await db.CompanyProvider.AsNoTracking().Where(p => p.CompanyId == id && p.ProviderId == 1)
            .Select(p => new { p.CompanyProviderId, p.ApiKey }).ToDictionaryAsync(p => p.CompanyProviderId, p => p.ApiKey, ct);
        using var client = clients.CreateClient();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(6));
        await Task.WhenAll(rows.Select(async row =>
        {
            row.Expired = row.ExpirationDate is { } expiry && expiry.Date < DateTime.UtcNow.AddHours(-5).Date;
            var section = configuration.GetValue<bool>("Enviroment:Production") ? "ApiPlemsi" : "ApiPlemsiQa";
            var setting = row.DocumentType == "POS" ? "PosLastInvoiceUrlMultipleResolution" : "FELastInvoiceUrlMultipleResolution";
            var url = configuration[$"{section}:{setting}"];
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(row.Prefix)
                || !keys.TryGetValue(row.CompanyProviderId, out var key) || string.IsNullOrWhiteSpace(key))
            {
                row.LookupStatus = "not_configured";
                return;
            }
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url + Uri.EscapeDataString(row.Prefix) + "&order=desc");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
                using var response = await client.SendAsync(request, timeout.Token);
                if (!response.IsSuccessStatusCode) { row.LookupStatus = "unavailable"; return; }
                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(timeout.Token));
                ApplyDocuments(row, json.RootElement);
            }
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or JsonException)
            {
                row.LookupStatus = "unavailable";
            }
        }));
        return Results.Ok(new { companyId = id, checkedAt = DateTimeOffset.UtcNow, resolutions = rows });
    }

    public static void ApplyDocuments(ResolutionStatus row, JsonElement root)
    {
        if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object
            || !data.TryGetProperty("docs", out var docs) || docs.ValueKind != JsonValueKind.Array)
        { row.LookupStatus = "invalid_response"; return; }
        // Plemsi returns documents in descending consecutive order. Never substitute another
        // resolution sharing this prefix, or a draft/rejected document, for this resolution.
        var matches = docs.EnumerateArray().Where(d => Text(d, "state") == "Emitted"
            && Text(d, "prefix") == row.Prefix && Text(d, "resolution_number") == row.ResolutionNumber)
            .Select(d => new { Document = d, Number = long.TryParse(Text(d, "number"), out var n) ? (long?)n : null })
            .Where(d => d.Number.HasValue).OrderByDescending(d => d.Number).ToList();
        if (matches.Count == 0) { row.LookupStatus = "not_found_in_latest_documents"; return; }
        var last = matches[0];
        row.LastInvoiceNumber = last.Number;
        // Preserve the document's date/time. It carries no timezone, and updatedAt can be
        // changed by later processing, so neither UTC nor an emission timestamp is invented.
        var date = Text(last.Document, "date");
        var time = Text(last.Document, "time");
        if (DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
            && TimeOnly.TryParse(time, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            row.LastInvoiceDateTime = date + " " + time;
        row.LookupStatus = "confirmed";
    }

    private static string? Text(JsonElement item, string name) => item.ValueKind == JsonValueKind.Object
        && item.TryGetProperty(name, out var value) && value.ValueKind is JsonValueKind.String or JsonValueKind.Number
            ? value.ToString() : null;
}

public sealed class ResolutionStatus
{
    public int ResolutionId { get; set; }
    public int CompanyProviderId { get; set; }
    public string DocumentType { get; set; } = "";
    public string? ResolutionNumber { get; set; }
    public string? Prefix { get; set; }
    public int? InitialRange { get; set; }
    public int? FinalRange { get; set; }
    public int? CurrentNumber { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? BillingUpdatedAt { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public bool Expired { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public long? LastInvoiceNumber { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string? LastInvoiceDateTime { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string LookupStatus { get; set; } = "unavailable";
}
