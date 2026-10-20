using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Api.Administration;

public sealed class OdooBillingLicenseAuthorizer(HttpClient http, IOptions<BillingAdministrationOptions> options)
    : IBillingLicenseAuthorizer
{
    public async Task<BillingLicenseDecision> AuthorizeAsync(int companyId, int companyProviderId, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (!settings.Enabled) return new(true, "legacy");
        if (!settings.IsConfigured) return new(false, "license_not_configured");
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, settings.LicenseUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
            // Odoo 19 selects a stateless multi-database request using this header, not ?db alone.
            var query = QueryHelpers.ParseQuery(new Uri(settings.LicenseUrl).Query);
            if (query.TryGetValue("db", out var database) && !string.IsNullOrWhiteSpace(database))
                request.Headers.Add("X-Odoo-Database", database.ToString());
            request.Content = JsonContent.Create(new { companyId, companyProviderId });
            using var response = await http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode) return new(false, "license_unavailable");
            var decision = await response.Content.ReadFromJsonAsync<BillingLicenseDecision>(cancellationToken);
            return decision is { Allowed: true } ? new(true, "active") : new(false, "license_inactive");
        }
        catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException
            || ex is TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            // No cached approval: an unreachable license authority must not enable invoicing.
            return new(false, "license_unavailable");
        }
    }
}
