using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.CreditNote.Ports;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CreditNote;

public class CreditNoteDomainService(IConfiguration config) : ICreditNoteDomainService
{
    public async Task<Result<ApiResponseCreditNote, Error>> Create(
        CreditNoteEntity creditNote,
        CancellationToken cancellationToken)
    {
        try
        {
            // Serializa en snake_case como espera Plemsi
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var jsonContent = JsonConvert.SerializeObject(creditNote, settings);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", creditNote.Customer.ApiKey);

            var url = bool.Parse(config["Enviroment:Production"]!)
                ? config["ApiPlemsi:CreateCreditNote"]
                : config["ApiPlemsiQa:CreateCreditNote"];

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = stringContent
            };

            var response = await client.SendAsync(httpRequest, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = Error.CreateInstance(
                    "CreditNote.SendError",
                    $"Error al enviar nota crédito a Plemsi. Status: {response.StatusCode}. Detalle: {content}",
                    response.StatusCode);
                return Result<ApiResponseCreditNote, Error>.Failure(error);
            }

            var responseApi = JsonConvert.DeserializeObject<ApiResponseCreditNote>(content);
            if (responseApi is null)
            {
                var error = Error.CreateInstance(
                    "CreditNote.DeserializeError",
                    "No se pudo deserializar la respuesta de Plemsi.",
                    System.Net.HttpStatusCode.InternalServerError);
                return Result<ApiResponseCreditNote, Error>.Failure(error);
            }

            return Result<ApiResponseCreditNote, Error>.Success(responseApi);
        }
        catch (Exception ex)
        {
            var error = Error.CreateInstance(
                "CreditNote.Exception",
                $"Excepción al procesar nota crédito: {ex.Message}",
                System.Net.HttpStatusCode.InternalServerError);
            return Result<ApiResponseCreditNote, Error>.Failure(error);
        }
    }
}
