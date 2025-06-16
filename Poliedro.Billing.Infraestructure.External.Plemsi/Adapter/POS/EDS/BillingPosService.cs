using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Application.Helper.EmailBuilder;
using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.SendEmail.Ports;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class BillingPosService(
    IInvoicePos _invoiceRepository,
    IInvoiceLastPos invoiceLastRepository,
    IDatabaseUtilsPos databaseUtils,
    IGetItemPos getItem,
    IGetItemsInvoicePos getItemsInvoicePos,
    IInsertInvoicePos insertInvoice,
    IUpdateCurrentlyNumber updateCurrentlyNumber,
    IEmailSender _emailNotificationService,
    IConfiguration config) : IBillingService
{
    public async Task<Result<ApiResponseBillingPos, Error>> CreateInvoicesPosAsync(
        IEnumerable<ClientEntity> Clients, CancellationToken cancellationToken)
    {
        List<BillingErrorDto> errorBilling = [];

        int invoice = 0;
        var billingResponse = new ApiResponseBillingPos();
        var ClientsPos = Clients.Where(c => c.DianResolution.ResolutionType == ResolutionType.POS && c.Active);
        foreach (var clientItem in ClientsPos)
        {
            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
            string connectionString = databaseUtils.GetConnectionString(clientItem.Server);
            IEnumerable<InvoiceEntity> invoicesFiltraded = await _invoiceRepository.GetInvoicesAsync(connectionString, cancellationToken, clientItem);
            if (invoicesFiltraded?.Count() > 0)
            {
                foreach (var item in invoicesFiltraded)
                {
                    invoice = 0;
                    int InvoiceNumber = int.Parse(item.Number[^6..]);
                    invoice = await invoiceLastRepository.GetInvoiceLastAsync(connectionString, clientItem, cancellationToken);
                    if (invoice == 0) return billingResponse;
                    DateTime date = DateTime.Now;
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    InvoicePosEntity data = new()
                    {
                        number = invoice,
                        date = formattedDate,
                        time = item.Time.ToString(),
                        softwareManufacturer = new SoftwareManufacturerEntity
                        {
                            ownerName = "Maicol Said Arevalo Gallardo",
                            softwareName = "Poliedro Pos",
                            companyName = "Poliedro Software S.A.S"
                        },
                        sendToEmail = "poliedrosoftware@gmail.com",
                        resolution = clientItem.DianResolution.ResolutionNumber,
                        prefix = clientItem.DianResolution.Prefix,
                        head_note = $"Fecha de la factura:{item.Date.ToString("yyyy-MM-dd")}",
                        foot_note = item.Number,
                        payment = new PaymentPosEntity
                        {
                            payment_form_id = 1,
                            payment_method_id = 30,
                            payment_due_date = formattedDate,
                            duration_measure = "1"
                        },
                        payPointInfo = new PayPointInfoEntity
                        {
                            code = "000001",
                            address = "Direccion Principal",
                            cashierName = "Cajero de Turno",
                            payPointType = "Caja Auxiliar",
                            saleCode = "V2398123",
                        },
                        invoiceBaseTotal = item.InvoiceBaseTotal.ToString(),
                        invoiceTaxExclusiveTotal = item.InvoiceTaxExclusiveTotal.ToString(),
                        invoiceTaxInclusiveTotal = item.InvoiceTaxInclusiveTotal.ToString(),
                        totalToPay = item.TotalToPay.ToString(),
                        allTaxTotals = [],
                        items = await getItemsInvoicePos.GetItemsInvoicePosAsync(InvoiceNumber, connectionString)
                    };
                    var jsonContent = JsonConvert.SerializeObject(data);
                    string CurrentlyDate = item.Date.ToString();
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    using var client = new HttpClient();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientItem.ApiKey);
                    var request = new HttpRequestMessage(HttpMethod.Post, bool.Parse(config["Enviroment:Production"]!) ? config["ApiPlemsi:PosUrl"] : config["ApiPlemsiQa:PosUrl"])
                    {
                        Content = stringContent
                    };

                    bool expirated = invoice > clientItem.DianResolution.FinalRange || DateTime.Now > clientItem.DianResolution.ExpirationDate;

                    if (expirated)
                    {
                        var message = EmailMessageBuilder.BuildResolutionExpiredMessage(
                            clientItem,
                            invoice,
                            config["EmailSettings:CompanyEmail"]!
                        );

                        await _emailNotificationService.SendEmailAsync(message);

                        await updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
                            new ParametersCurrentlyNumber(invoice, CurrentlyDate, clientItem.ResolutionId, true),
                           cancellationToken);
                    }


                    HttpResponseMessage response = !expirated ? await client.SendAsync(request) : new HttpResponseMessage();
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseApi = JsonConvert.DeserializeObject<ApiResponseBillingPos>(jsonResponse);
                        if (responseApi is not null && responseApi.Success)
                        {
                            await insertInvoice.InsertInvoiceSucces(
                                invoice,
                                responseApi.Data.Cude,
                                responseApi.Data.QRCode,
                                connectionString,
                                clientItem.ProviderId,
                                clientItem.ClientBillingElectronicId,
                                item.Resolution);

                            await updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
                             new ParametersCurrentlyNumber(invoice, CurrentlyDate, clientItem.ResolutionId, true),
                            cancellationToken);
                        }
                    }
                    else
                    {
                        string? errorContent = response.Content is not null
                            ? await response.Content.ReadAsStringAsync()
                            : "No content";

                        errorBilling.Add(new BillingErrorDto(
                        clientItem.DianResolution.Prefix,
                        invoice.ToString(),
                        clientItem.Name,
                         $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}: {errorContent}"
                        ));

                    }
                }
            }
        }
        if (errorBilling.Any())
        {
            var message = FailedBillingMessageBuilder.MessageBuilder(
                errorBilling,
                config["EmailSettings:CompanyEmail"]!);

            await _emailNotificationService.SendEmailAsync(message);
        }
        return billingResponse;
    }
}



