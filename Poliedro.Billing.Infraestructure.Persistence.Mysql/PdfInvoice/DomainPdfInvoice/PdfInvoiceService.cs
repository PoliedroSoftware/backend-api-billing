using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.PdfInvoice.Service;
using System.Text.Json;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.PdfInvoice.DomainPdfInvoice;

public class PdfInvoiceService : IPdfInvoiceService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiUrl;

    public PdfInvoiceService(IConfiguration configuration)
    {
        _apiUrl = configuration["ApiPlemsi:PdfInvoiceUrl"]
                  ?? throw new ArgumentNullException("PdfInvoiceUrl is missing in appsettings.json");
    }

    public async Task<byte[]> GetPdfBytes(string id, string bearerToken)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty.");

        try
        {
            string base64Pdf = await DownloadPdfAsBase64Async(id, bearerToken);
            return Convert.FromBase64String(base64Pdf);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error fetching PDF as bytes: {ex.Message}", ex);
        }
    }

    private async Task<string> DownloadPdfAsBase64Async(string id, string bearerToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var pdfObject = JsonSerializer.Deserialize<PdfResponse>(jsonResponse);

            if (pdfObject == null || string.IsNullOrEmpty(pdfObject.data))
            {
                throw new InvalidOperationException("La respuesta de la API no contiene datos válidos.");
            }

            return pdfObject.data;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Error fetching PDF from API: {ex.Message}", ex);
        }
    }
}

public class PdfResponse
{
    public string data { get; set; }
}