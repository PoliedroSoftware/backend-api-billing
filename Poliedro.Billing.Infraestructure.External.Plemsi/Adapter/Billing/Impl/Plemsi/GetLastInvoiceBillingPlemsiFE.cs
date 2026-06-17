using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

public class GetLastInvoiceBillingPlemsiFE(
    IConfiguration config
    ) : IGetLastInvoiceBilling
{
    private static readonly HttpClient Client = new();
    public async Task<int> GetLastInvoiceNumberAsync(DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken CancellationToken)
    {
        int MaxNumeroFactura = 1;
        DateTime ToDay = DateTime.Now;
        string FormattedDate = ToDay.ToString("yyyy-MM-dd");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", companyProviderEntity.ApiKey);
        string ApiUrl = string.Empty;

        bool isProduction = bool.Parse(config["Enviroment:Production"]!);
        string baseUrl;

        if (!Enum.TryParse(dianResolutionEntity.MultipleResolution.ToString(), out MultipleResolution resolution))
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

        ApiUrl = $"{baseUrl}{dianResolutionEntity.Prefix}";

        HttpResponseMessage Response = await Client.GetAsync(ApiUrl);

        if (Response.IsSuccessStatusCode)
        {
            try
            {
                string JsonResponse = await Response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(JsonResponse);
                var Documents = jObject["data"]?["docs"];

                if (Documents != null && Documents.HasValues)
                {
                    int maxEmitted = 0;
                    int maxDeleted = 0;

                    foreach (var Doc in Documents)
                    {
                        string? state = Doc["state"]?.ToString();
                        string? numberStr = Doc["number"]?.ToString();

                        if (int.TryParse(numberStr, out int num))
                        {
                            if (state == "Emitted" && num > maxEmitted)
                            {
                                maxEmitted = num;
                            }

                            if (state == "Deleted" && num > maxDeleted)
                            {
                                maxDeleted = num;
                            }
                        }
                    }

                    int maxUsed = Math.Max(maxEmitted, maxDeleted);

                    if (maxUsed > 0)
                    {
                        return maxUsed + 1;
                    }
                }
            }
            catch (Exception)
            {
                return MaxNumeroFactura + 1;
            }
        }

        return MaxNumeroFactura;

    }
}
