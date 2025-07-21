using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

public class BillingSenderFE(IConfiguration config) : IBillingSender
{
    public async Task SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
    {
        //var FeInvoices = invoices.Cast<PlemiFEInvoiceDTO>().ToList();

        var jsonContent = JsonConvert.SerializeObject(request.Invoices);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.ApiKey);

        var requestPlemsi = new HttpRequestMessage(HttpMethod.Post, bool.Parse(config["Enviroment:Production"]!) ? config["ApiPlemsi:FEUrl"] : config["ApiPlemsiQa:FEUrl"])
        {
            Content = stringContent
        };

        var response = await client.SendAsync(requestPlemsi, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al enviar a PLEMSI: {response.StatusCode} - {content}");
        }

        await Task.CompletedTask;
    }
}