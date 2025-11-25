
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using System.Net.Http.Headers;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

public class GetLastInvoiceNumberCreditNotePlemsi(
    IConfiguration _config
    ) : IGetLastInvoiceNumberCreditNote
{
    private static readonly HttpClient Client = new();
    public async Task<int> GetLastInvoiceNumberCreditNoteAsync(BillingInfoClient clientInfo, CancellationToken cancellationToken)
    {
        int MaxNumberInvoice = 1;
        DateTime ToDay = DateTime.Now;
        string FormattedDate = ToDay.ToString("yyyy-MM-dd");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientInfo.ApiKey);
        string ApiUrl = string.Empty;

        bool isProduction = bool.Parse(_config["Enviroment:Production"]!);
        string baseUrl;

        if (!Enum.TryParse(clientInfo.MultipleResolution.ToString(), out MultipleResolution resolution))
        {
            resolution = MultipleResolution.Single;
        }

        switch (resolution)
        {
            case MultipleResolution.Multiple:
                baseUrl = isProduction
                    ? _config["ApiPlemsi:NCLastInvoiceUrl"]
                    : _config["ApiPlemsiQa:"];
                break;

                default:
                baseUrl = isProduction
                    ? _config["ApiPlemsi:NCLastInvoiceUrl"]
                    : _config["ApiPlemsiQa:"];
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
                return MaxNumberInvoice + 1;
            }
        }

        return MaxNumberInvoice;

    }
}
