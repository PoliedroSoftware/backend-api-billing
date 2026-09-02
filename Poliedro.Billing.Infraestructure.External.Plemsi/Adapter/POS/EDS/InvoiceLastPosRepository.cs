using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class InvoiceLastPosRepository(IConfiguration config, IHttpClientFactory httpClientFactory): IInvoiceLastPos
{
    public async Task<int> GetInvoiceLastAsync(
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken cancellationToken)
    {
<<<<<<< HEAD
        using var client = httpClientFactory.CreateClient();

        bool isProduction = bool.Parse(config["Enviroment:Production"]!);

        string baseUrl;

        if (!Enum.TryParse(dianResolutionEntity.MultipleResolution.ToString(),
=======

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientInfo.ApiKey);

        bool isProduction = bool.Parse(config["Enviroment:Production"]!);

        string baseUrl;

        if (!Enum.TryParse(clientInfo.MultipleResolution.ToString(),
>>>>>>> origin/releasecandidate/v1.0.0
            out MultipleResolution resolution))
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

<<<<<<< HEAD
        string ApiUrl = $"{baseUrl}{dianResolutionEntity.Prefix}";

        var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", companyProviderEntity.ApiKey);

        HttpResponseMessage Response =
            await client.SendAsync(request, cancellationToken);

=======
        string ApiUrl = $"{baseUrl}{clientInfo.Prefix}";

        HttpResponseMessage Response =
            await client.GetAsync(ApiUrl, cancellationToken);

>>>>>>> origin/releasecandidate/v1.0.0
        if (!Response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"No fue posible consultar el consecutivo en Plemsi. " +
                $"HTTP {(int)Response.StatusCode} - {Response.ReasonPhrase}");
        }

        try
        {
            string JsonResponse = await Response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(JsonResponse);
            var Documents = jObject["data"]?["docs"];

            if (Documents == null || !Documents.HasValues)
            {
                throw new InvalidOperationException(
                    "Plemsi respondió correctamente, pero no se encontraron documentos para determinar el consecutivo.");
            }

            int maxUsed = 0;

            foreach (var Doc in Documents)
            {
                string? numberStr = Doc["number"]?.ToString();

                if (int.TryParse(numberStr, out int num) && num > maxUsed)
                {
                    maxUsed = num;
                }
            }

            if (maxUsed <= 0)
            {
                throw new InvalidOperationException("No fue posible obtener el último consecutivo de facturación desde Plemsi.");
            }

            return maxUsed + 1;

<<<<<<< HEAD
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
        "La respuesta de Plemsi no tiene un formato JSON válido.",
        ex);
        }
=======
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
        "La respuesta de Plemsi no tiene un formato JSON válido.",
        ex);
        }



>>>>>>> origin/releasecandidate/v1.0.0
    }


}
