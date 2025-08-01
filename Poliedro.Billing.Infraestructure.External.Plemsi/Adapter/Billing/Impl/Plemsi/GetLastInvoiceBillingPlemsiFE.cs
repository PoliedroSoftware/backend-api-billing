using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

public class GetLastInvoiceBillingPlemsiFE(
    IConfiguration config,
    IUpdateCurrentlyNumber _updateCurrentlyNumber
    ) : IGetLastInvoiceBilling
{
    private static readonly HttpClient Client = new();
    public async Task<int> GetLastInvoiceNumberAsync(BillingInfoClient clientInfo, CancellationToken CancellationToken)
    {
        int MaxNumeroFactura = 1;
        DateTime ToDay = DateTime.Now;
        string FormattedDate = ToDay.ToString("yyyy-MM-dd");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientInfo.ApiKey);
        string ApiUrl = string.Empty;

        bool isProduction = bool.Parse(config["Enviroment:Production"]!);
        string baseUrl;

        if (!Enum.TryParse(clientInfo.MultipleResolution.ToString(), out MultipleResolution resolution))
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

        ApiUrl = $"{baseUrl}{clientInfo.Prefix}";

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
                    foreach (var Doc in Documents)
                    {
                        Console.WriteLine(Doc["state"]?.ToString());

                        if (Doc["state"]?.ToString() == "Emitted")
                        {
                            if (int.TryParse(Doc["number"]?.ToString(), out int ParsedNumber))
                            {
                                return ParsedNumber + 1;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return MaxNumeroFactura + 1;
            }

            int CurrentlyNumber = clientInfo.CurrentlyNumber;

            if (CurrentlyNumber > MaxNumeroFactura)
            {
                MaxNumeroFactura = CurrentlyNumber;
            }
            else
            {

                var ParametersCurrentlyNumber = new ParametersCurrentlyNumber(
                    Invoice: MaxNumeroFactura,
                    CurrentlyDate: null,
                    ResolutionId: clientInfo.ResolutionId,
                    Expirated: false
                );


                await _updateCurrentlyNumber.UpdateCurrentlyNumberAsync(ParametersCurrentlyNumber, CancellationToken);
            }


        }
        return MaxNumeroFactura;

    }
}
