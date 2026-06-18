using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi.POS;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

public class BillingSenderPOS(
    IConfiguration config,
    IInvoiceLastPos _getLastInvoiceBilling
    ) : IBillingSender

{
    public async Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
    {
        var responses = new List<ApiResponseFERetailPos>();

        foreach (var invoice in request.Invoices)
        {
            try
            {
                InvoiceRequestPosDto? invoiceRequestDto = invoice as InvoiceRequestPosDto;  

                int lastNumber = await _getLastInvoiceBilling.GetInvoiceLastAsync(request.DianResolutionEntity,request.CompanyProviderEntity, cancellationToken);

                if (invoiceRequestDto.number < lastNumber)
                {
                    invoiceRequestDto.number = lastNumber;
                    
                }

                var jsonContent = JsonConvert.SerializeObject(invoiceRequestDto);
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.CompanyProviderEntity.ApiKey);

                var url = bool.Parse(config["Enviroment:Production"]!)
                    ? config["ApiPlemsi:PosUrl"]
                    : config["ApiPlemsiQa:PosUrl"];

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
                Console.WriteLine($"[ERROR] Excepcion procesando factura: {ex.Message}");
                continue;
            }
        }

        return responses;
    }
}
