using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;
using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.GetInvoice;

public class GetInvoiceDomainGetInvoice(IConfiguration config) : IGetInvoiceDomainGetInvoice
{
    public async Task<GetInvoiceEntity?> GetByIdAsync(string cufe, string token, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var bearerToken = token.Trim();
        if (bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            bearerToken = bearerToken[7..].Trim();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var baseUrl = config["ApiPlemsi:GetInvoice"]
            ?? throw new InvalidOperationException("ApiPlemsi:GetInvoice configuration is missing");
        using var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + Uri.EscapeDataString(cufe.Trim()));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var response = await client.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<GetInvoiceApiResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        // Invalid payloads are failures, never evidence that an invoice does not exist.
        if (result is not { Success: true, Data: not null })
            throw new InvalidOperationException("Plemsi did not return a valid invoice detail");
        return result.Data;
    }
}
