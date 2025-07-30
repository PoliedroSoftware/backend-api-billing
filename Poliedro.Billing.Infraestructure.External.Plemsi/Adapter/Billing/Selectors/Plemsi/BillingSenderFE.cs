using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

public class BillingSenderFE(IConfiguration config) : IBillingSender
{
    public async Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
    {
        var responses = new List<ApiResponseFERetailPos>();

        foreach (var invoice in request.Invoices)
        {

            var jsonContent = JsonConvert.SerializeObject(invoice);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.ApiKey);

            var url = bool.Parse(config["Enviroment:Production"]!)
            ? config["ApiPlemsi:FEUrl"]
            : config["ApiPlemsiQa:FEUrl"];

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = stringContent
            };

            var response = await client.SendAsync(httpRequest, cancellationToken);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al enviar a PLEMSI: {response.StatusCode} - {content}");
            }

            var responseApi = JsonConvert.DeserializeObject<ApiResponseFERetailPos>(content);
            if (responseApi is null)
                throw new Exception("No se pudo deserializar la respuesta de PLEMSI.");

            responses.Add(responseApi);
        }
        return responses;

    }

}
