using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.GetInvoice.DomainGetInvoice;
using Poliedro.Billing.Domain.GetInvoice.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.GetInvoice;

public class GetInvoiceDomainGetInvoice(IConfiguration config, HttpClient client) : IGetInvoiceDomainGetInvoice
{
    public async Task<GetInvoiceEntity?> GetByIdAsync(string cufe, string token, CancellationToken cancellationToken)
    {
        try
        {
            string bearerToken = token.Trim();
            if (!bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                bearerToken = $"Bearer {bearerToken}";
            }

            var authorization = new AuthenticationHeaderValue("Bearer", bearerToken.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim());

            string baseUrl = config["ApiPlemsi:GetInvoice"] ?? throw new InvalidOperationException("ApiPlemsi:GetInvoice configuration is missing");
            string url = baseUrl + cufe.Trim();


            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = authorization;

            var response = await client.SendAsync(request, cancellationToken);


            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var apiResponse = JsonSerializer.Deserialize<GetInvoiceApiResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            return null;
        }
    }
}
