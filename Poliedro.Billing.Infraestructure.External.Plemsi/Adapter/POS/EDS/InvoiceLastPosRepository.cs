using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class InvoiceLastPosRepository(IConfiguration config): IInvoiceLastPos
{
    private static readonly HttpClient client = new();

    public async Task<int> GetInvoiceLastAsync(
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken cancellationToken)
    {
        int maxNumeroFactura = 1;
        DateTime today = DateTime.Now;
        string formattedDate = today.ToString("yyyy-MM-dd");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", companyProviderEntity.ApiKey);

        var isProduction = bool.Parse(config["Enviroment:Production"]!);
        string baseUrl;

        if (!Enum.TryParse(dianResolutionEntity.MultipleResolution.ToString(), out MultipleResolution resolution))
        {
            resolution = MultipleResolution.Single;
        }

        switch (resolution)
        {
            case MultipleResolution.Multiple:
                baseUrl = isProduction
                    ? config["ApiPlemsi:PosLastInvoiceUrlMultipleResolution"]
                    : config["ApiPlemsiQa:PosLastInvoiceUrlMultipleResolution"];
                break;

            default:
                baseUrl = isProduction
                    ? config["ApiPlemsi:PosLastInvoiceUrl"]
                    : config["ApiPlemsiQa:PosLastInvoiceUrl"];
                break;
        }


        string apiUrl = $"{baseUrl}{dianResolutionEntity.Prefix}";

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
