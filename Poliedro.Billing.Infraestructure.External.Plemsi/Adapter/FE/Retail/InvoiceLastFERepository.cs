using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
public class InvoiceLastFERepository(IConfiguration config) : IInvoiceLastFE
{
    private static readonly HttpClient client = new();

    public async Task<int> GetInvoiceLastAsync(string connectionString, ClientEntity clientItem, CancellationToken cancellationToken)
    {
        int maxNumeroFactura = 1;
        DateTime today = DateTime.Now;
        string formattedDate = today.ToString("yyyy-MM-dd");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientItem.ApiKey);
        string apiUrl = string.Empty;
        if ((MultipleResolution)clientItem.MultipleResolution == MultipleResolution.Multiple)
        {
            string baseUrl = bool.Parse(config["Enviroment:Production"]!) ? config["ApiPlemsi:FELastInvoiceUrlMultipleResolution"] : config["ApiPlemsiQa:FELastInvoiceUrlMultipleResolution"];
            apiUrl = $"{baseUrl}{clientItem.DianResolution.Prefix}";
        }
        else
        {
            string baseUrl = bool.Parse(config["Enviroment:Production"]!) ? config["ApiPlemsi:FELastInvoiceUrl"] : config["ApiPlemsiQa:FELastInvoiceUrl"];
            apiUrl = $"{baseUrl}{clientItem.DianResolution.Prefix}";
        }
            HttpResponseMessage response = await client.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            try
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(jsonResponse);
                var documents = jObject["data"]?["docs"];
                if (documents != null && documents.HasValues)
                {
                    foreach (var doc in documents)
                    {
                        Console.WriteLine(doc["state"]?.ToString());

                        if (doc["state"]?.ToString() == "Emitted")
                        {
                            if (int.TryParse(doc["number"]?.ToString(), out int parsedNumber))
                            {
                                return parsedNumber + 1;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return maxNumeroFactura + 1;
            }

        }
        return maxNumeroFactura;
    }
}

