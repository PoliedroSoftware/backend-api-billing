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

            int maxUsed = 0;

            foreach (var Doc in documents)
            {
                // Plemsi can return rejected attempts or documents from other resolutions.
                // An emitted invoice consumes its number; a later provider deletion does not
                // release a number once Plemsi assigned it a CUDE.
                if (Doc["state"] is null || Doc["prefix"] is null || Doc["resolution_number"] is null)
                    throw new InvalidOperationException("Plemsi devolvió un documento sin datos suficientes para validar la resolución.");

                bool consumed = string.Equals(Doc["state"]?.ToString(), "Emitted", StringComparison.OrdinalIgnoreCase)
                    || !string.IsNullOrWhiteSpace(Doc["cude"]?.ToString());
                if (!consumed
                    || !string.Equals(Doc["prefix"]?.ToString(), dianResolutionEntity.Prefix, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(Doc["resolution_number"]?.ToString(), dianResolutionEntity.ResolutionNumber, StringComparison.Ordinal))
                    continue;

                string? numberStr = Doc["number"]?.ToString();
                if (!int.TryParse(numberStr, out int num) || num <= 0)
                    throw new InvalidOperationException("Plemsi devolvió una factura emitida sin consecutivo válido.");
                maxUsed = Math.Max(maxUsed, num);
            }

            if (maxUsed <= 0)
            {
                if (dianResolutionEntity.InitialRange <= 0)
                    throw new InvalidOperationException("La resolución no tiene un rango inicial válido.");
                return dianResolutionEntity.InitialRange;
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

