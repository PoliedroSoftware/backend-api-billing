using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

public class GetLastInvoiceBillingPlemsi(
    IConfiguration config,
    IHttpClientFactory httpClientFactory
    
    ) : IGetLastInvoiceBilling
{
    public async Task<int>  GetLastInvoiceNumberAsync(DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken)
    {
        using var client = httpClientFactory.CreateClient();

        bool isProduction = bool.Parse(config["Enviroment:Production"]!);

        string baseUrl;

        if (!Enum.TryParse(dianResolutionEntity.MultipleResolution.ToString(),
            out MultipleResolution resolution))
        {
            resolution = MultipleResolution.Single;
        }

        switch (resolution)
        {
            case MultipleResolution.Multiple:
                baseUrl = isProduction
                    ? config["ApiPlemsi:FELastInvoiceUrlMultipleResolution"]
                    : config["ApiPlemsiQa:FELastInvoiceUrlMultipleResolution"];
                break;

            default:
                baseUrl = isProduction
                    ? config["ApiPlemsi:FELastInvoiceUrl"]
                    : config["ApiPlemsiQa:FELastInvoiceUrl"];
                break;
        }

        string ApiUrl = $"{baseUrl}{dianResolutionEntity.Prefix}";

        var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", companyProviderEntity.ApiKey);

        HttpResponseMessage Response =
            await client.SendAsync(request, cancellationToken);

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

            if (Documents is not JArray documents)
            {
                throw new InvalidOperationException("Plemsi no devolvió una lista de documentos válida.");
            }

            if (documents.Count == 0)
            {
                // Resolución sin documentos emitidos: el primer consecutivo es el rango inicial.
                if (dianResolutionEntity.InitialRange <= 0)
                    throw new InvalidOperationException("La resolución no tiene un rango inicial válido.");
                return dianResolutionEntity.InitialRange;
            }

            int maxUsed = 0;

            foreach (var Doc in documents)
            {
                string? numberStr = Doc["number"]?.ToString();

                if (!int.TryParse(numberStr, out int num) || num <= 0)
                    throw new InvalidOperationException("Plemsi devolvió un documento sin consecutivo válido.");
                maxUsed = Math.Max(maxUsed, num);
            }

            if (maxUsed <= 0)
            {
                throw new InvalidOperationException("No fue posible obtener el último consecutivo de facturación desde Plemsi.");
            }

            return checked(maxUsed + 1);

        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
        "La respuesta de Plemsi no tiene un formato JSON válido.",
        ex);
        }

    }
}

