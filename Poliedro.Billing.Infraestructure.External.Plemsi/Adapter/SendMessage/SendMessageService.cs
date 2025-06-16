using Newtonsoft.Json;
using Poliedro.Billing.Domain.FERetail.Ports;
using System.Net.Http.Headers;
using System.Text;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage;

public class SendMessageService : ISendMessage
{
    public async Task SendMessageAsync(string Invoice, string Customer)
    {
        var client = new HttpClient();
        var messageContent = new
        {
            phone = "+573157690579",
            message = $"Hola {Customer}, tu factura número {Invoice} fue emitida a la Dian."
        };

        var jsonContent = JsonConvert.SerializeObject(messageContent);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.wiivo.net/v1/messages?Token=a665484ada205840bd8d1fc04b3495138a5bf86c2adba37cf3513fd1b86e2bdd076f1ed37aea811f"),
            Headers =
            {
                { "Token", "a665484ada205840bd8d1fc04b3495138a5bf86c2adba37cf3513fd1b86e2bdd076f1ed37aea811f" },
            },
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
        }
    }
}