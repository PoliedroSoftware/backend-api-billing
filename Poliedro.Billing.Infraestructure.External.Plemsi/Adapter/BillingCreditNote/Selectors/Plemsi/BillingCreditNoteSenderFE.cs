
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.BillingCreditNote.Selectors.Plemsi;

public class BillingCreditNoteSenderFE(
        IConfiguration _config,
    IGetLastInvoiceNumberCreditNote _getLastInvoiceNumberCreditNote
    ) : IBillingCreditNoteSender
{
    public async Task<List<ApiResponseFERetailPos>> SendCreditNoteAsync(
        PlemsiInvoiceRequest request,
        BillingInfoClient clientInfo,
        CancellationToken cancellationToken)
    {
        var responses = new List<ApiResponseFERetailPos>();

        foreach (var invoice in request.Invoices)
        {
            try
            {
                CreditNoteDTO? SenderRequestDTO = invoice as CreditNoteDTO;

                int LastNumberCreditNote = await _getLastInvoiceNumberCreditNote.GetLastInvoiceNumberCreditNoteAsync(clientInfo, cancellationToken);

                if(SenderRequestDTO.number < LastNumberCreditNote)
                {
                    SenderRequestDTO.number = LastNumberCreditNote;
                }

                var jsonContent = JsonConvert.SerializeObject(invoice);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.ApiKey);

                var url = bool.Parse(_config["Enviroment:Production"]!)
                ? _config["ApiPlemsi:CreateCreditNote"]
                : _config["ApiPlemsiQa:CreateCreditNote"];

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = stringContent
                };

                var response = await client.SendAsync(httpRequest, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] Envio fallido. Status: {response.StatusCode}. Contenido: {content}");
                    continue;
                }

                var responseApi = JsonConvert.DeserializeObject<ApiResponseFERetailPos>(content);
                if (responseApi is null)
                {
                    Console.WriteLine($"[ERROR] No se pudo deserializar la respuesta: {content}");
                    continue;
                }

                responses.Add(responseApi);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Excepción procesando factura: {ex.Message}");
                continue;
               
            }
        }
        return responses;

    }
}
