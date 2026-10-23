using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Domain.FERetail.Ports;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage;

public sealed class SendMessageService(HttpClient client, IConfiguration configuration) : ISendMessage
{
    public async Task SendMessageAsync(string invoice, string customer)
    {
        var endpoint = configuration["WhatsApp:Endpoint"];
        var token = configuration["WhatsApp:Token"];
        var phone = configuration["WhatsApp:Phone"];
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
            || string.IsNullOrWhiteSpace(token)
            || string.IsNullOrWhiteSpace(phone))
            throw new InvalidOperationException("WhatsApp endpoint, token and destination phone must be configured.");

        var content = JsonConvert.SerializeObject(new
        {
            phone,
            message = $"Hola {customer}, tu factura número {invoice} fue emitida a la Dian."
        });
        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
        request.Headers.TryAddWithoutValidation("Token", token);

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
