using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.InvoicePos.Entities;
using Poliedro.Billing.Domain.InvoicePos.Ports;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicePos;

public class InvoicePosRepository : IInvoicePosRepository
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public InvoicePosRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<IEnumerable<InvoicePosEntity>> GetAllAsync(string? prefix, CancellationToken cancellationToken)
    {
        try 
        {
            var apiUrl = _configuration["ApiPlemsi:PosUrl"];

            if (string.IsNullOrEmpty(apiUrl))
            {
                throw new InvalidOperationException("ApiPlemsi URL configuration is missing. Please check appsettings.json");
            }

            string apiKey = prefix switch
            {
                "POSC" => "b57ea734c46bff7e03cfb085",
                "POSG" => "126cffeb17ffc881acb1dba4",
                _ => _configuration["ApiPlemsi:ApiKey"] ?? throw new InvalidOperationException("No API key found for the specified prefix")
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var url = $"{apiUrl}?page=1&perPage=100";
            if (!string.IsNullOrEmpty(prefix))
            {
                url += $"&search={prefix}";
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Error calling Plemsi API. Status: {response.StatusCode}, Content: {errorContent}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var jObject = JObject.Parse(jsonResponse);
            var documents = jObject["data"]?["docs"];

            if (documents == null || !documents.HasValues)
            {
                return Enumerable.Empty<InvoicePosEntity>();
            }

            var invoices = new List<InvoicePosEntity>();
            foreach (var doc in documents)
            {
                try
                {
                    var responseData = new Response();
                    var docResponse = doc["response"];
                    if (docResponse != null)
                    {
                        responseData.message = docResponse["message"]?.ToString() ?? string.Empty;
                        var data = docResponse["data"];
                        if (data != null)
                        {
                            responseData.data = new ResponseData
                            {
                                IsValid = data["IsValid"]?.ToString() ?? string.Empty,
                                StatusDescription = data["StatusDescription"]?.ToString() ?? string.Empty,
                                IssueDate = data["IssueDate"]?.ToString() ?? string.Empty,
                                IssueTime = data["IssueTime"]?.ToString() ?? string.Empty,
                                Filename = data["Filename"]?.ToString() ?? string.Empty,
                                ErrorMessage = new ErrorMessage
                                {
                                    @string = data["ErrorMessage"]?["string"]?.ToString() ?? string.Empty
                                }
                            };
                        }
                    }

                    var invoice = new InvoicePosEntity
                    {
                        _id = doc["_id"]?.ToString() ?? string.Empty,
                        company = doc["company"]?.ToString() ?? string.Empty,
                        state = doc["state"]?.ToString() ?? string.Empty,
                        generatedBy = doc["generatedBy"]?.ToString() ?? string.Empty,
                        resolution = doc["resolution"]?.ToString() ?? string.Empty,
                        type_document_id = doc["type_document_id"]?.Value<int>() ?? 0,
                        date = doc["date"]?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd"),
                        time = doc["time"]?.ToString() ?? "00:00:00",
                        notes = doc["notes"]?.ToString() ?? string.Empty,
                        head_note = doc["head_note"]?.ToString() ?? string.Empty,
                        foot_note = doc["foot_note"]?.ToString() ?? string.Empty,
                        resolution_number = doc["resolution_number"]?.ToString() ?? string.Empty,
                        prefix = doc["prefix"]?.ToString() ?? string.Empty,
                        number = doc["number"]?.ToString() ?? string.Empty,
                        consecutive = doc["consecutive"]?.ToString() ?? string.Empty,
                        QRStr = doc["QRStr"]?.ToString() ?? string.Empty,
                        cude = doc["cude"]?.ToString() ?? string.Empty,
                        createdAt = doc["createdAt"]?.Value<DateTime>() ?? DateTime.Now,
                        updatedAt = doc["updatedAt"]?.Value<DateTime>() ?? DateTime.Now,
                        tax_totals = doc["tax_totals"]?.ToObject<List<TaxTotal>>() ?? new List<TaxTotal>(),
                        with_holding_tax_total = doc["with_holding_tax_total"]?.ToObject<List<object>>() ?? new List<object>(),
                        invoice_lines = doc["invoice_lines"]?.ToObject<List<InvoiceLine>>() ?? new List<InvoiceLine>(),
                        allowance_charges = doc["allowance_charges"]?.ToObject<List<object>>() ?? new List<object>(),
                        legal_monetary_totals = doc["legal_monetary_totals"]?.ToObject<LegalMonetaryTotals>() ?? new LegalMonetaryTotals(),
                        payment_form = doc["payment_form"]?.ToObject<PaymentForm>() ?? new PaymentForm(),
                        response = responseData,
                        additionalData = doc["additionalData"]?.ToObject<AdditionalData>() ?? new AdditionalData()
                    };
                    invoices.Add(invoice);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error parsing invoice data: {ex.Message} for document: {doc["number"]}", ex);
                }
            }
            return invoices;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetAllAsync: {ex.Message}", ex);
        }
    }
}
