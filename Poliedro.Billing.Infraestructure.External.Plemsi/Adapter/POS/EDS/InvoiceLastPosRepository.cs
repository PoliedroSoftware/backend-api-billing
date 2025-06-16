using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.Entities;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class InvoiceLastPosRepository(IConfiguration config): IInvoiceLastPos
{
    private static readonly HttpClient client = new();

    public async Task<int> GetInvoiceLastAsync(string connectionString, ClientEntity clientEntity, CancellationToken cancellationToken)
    {
        int maxNumeroFactura = 1;
        DateTime today = DateTime.Now;
        string formattedDate = today.ToString("yyyy-MM-dd");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientEntity.ApiKey);
        var url = bool.Parse(config["Enviroment:Production"]!) ? config["ApiPlemsi:PosLastInvoiceUrl"] : config["ApiPlemsiQa:PosLastInvoiceUrl"];
        string apiUrl = $"{url}{clientEntity.DianResolution.Prefix}";
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


            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la respuesta: {ex.Message}");
            }
        }
        return maxNumeroFactura;
    }
}
