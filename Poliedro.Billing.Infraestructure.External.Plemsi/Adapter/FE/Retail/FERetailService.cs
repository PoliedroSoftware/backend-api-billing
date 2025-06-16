using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poliedro.Billing.Application.Helper.DigitalVerification;
using Poliedro.Billing.Application.Helper.EmailBuilder;
using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.SendEmail.Ports;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using System.Net.Http.Headers;
using System.Text;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
public class FERetailService(
IInvoiceFE _invoiceRepository,
IInvoiceLastFE invoiceLastRepository,
IDatabaseUtils databaseUtils,
IGetItemsInvoiceFERetail getItemsInvoiceFERetail,
IInsertInvoiceFE insertInvoice,
IUpdateCurrentlyNumber updateCurrentlyNumber,
IEmailSender _emailNotificationService,
IConfiguration config) : IFERetailService
{
    public async Task<Result<ApiResponseFERetailPos, Error>> CreateElectronicInvoicesAsync(
        IEnumerable<ClientEntity> Clients, CancellationToken cancellationToken)
    {
        var FERetailResponse = new ApiResponseFERetailPos() { Code = 200, Data = new apiDataPos(), Info = "ok", Success = true };
        var ClientsFE = Clients.Where(c => c.DianResolution.ResolutionType == ResolutionType.FE && c.Active);
        List<BillingErrorDto> errorBilling = [];

        foreach (var clientItem in ClientsFE)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            var connectionString = databaseUtils.GetConnectionString(clientItem.Server);
            IEnumerable<InvoiceEntity> invoicesFiltraded = await _invoiceRepository.GetInvoicesAsync(connectionString, cancellationToken, clientItem);
            if (invoicesFiltraded?.Count() > 0)
            {
                foreach (var item in invoicesFiltraded)
                {

                    List<ItemElectronicEntity> Itens = await getItemsInvoiceFERetail.GetItemsInvoiceFERetailAsync(item.Id.ToString(), connectionString);
                    double totalToBase = Itens?.Sum(item => item.line_extension_amount) ?? 0;
                    double totalTaxableAmount = Itens?.Sum(item => item.tax_totals.Sum(tax => tax.tax_amount)) ?? 0;
                    double totalBaseGravable = Itens?.Sum(item => item.tax_totals.Sum(tax => tax.taxable_amount)) ?? 0;
                    double totalToPay = totalToBase + totalTaxableAmount;
                    List<AllTaxTotalEntity> allTaxTotals = [];

                    allTaxTotals = GetAllTaxTotals(Itens);

                    int InvoiceNumber = int.Parse(item.invoice[^4..]);
                    int invoice = await invoiceLastRepository.GetInvoiceLastAsync(connectionString, clientItem, cancellationToken);
                    if (invoice == 0) return FERetailResponse;
                    if (invoice == 1) invoice = clientItem.DianResolution.InitialRange;
                    if (invoice < clientItem.DianResolution.InitialRange) invoice = clientItem.DianResolution.InitialRange;
                    DateTime date = DateTime.Now;
                    string formattedDate = date.ToString("yyyy-MM-dd");
                    string currentTime = date.ToString("HH:mm:ss");
                    var documentType = ValidarGuion(item.identication);


                    if (documentType == DocumentType.NIT)
                    {
                        item.identication = item.identication.Replace("-", "");
                        if (item.identication.Length > 0)
                        {
                            item.identication = item.identication.Substring(0, item.identication.Length - 1);
                        }
                    }
                    var identification = item.identication.Trim().Replace(".", "").Replace("-", "").Replace(" ", "").Replace("+", "");
                    var dv = DigitalVerification.CalcularDigitoVerificacion(identification) ?? "1";
                    if (dv == "error")
                    {
                        identification = config["CosumerFinal:identification"];
                        dv = config["CosumerFinal:dv"];
                    }

                    FERetailelectronicEntity data = new()
                    {
                        date = formattedDate,
                        time = currentTime,
                        prefix = clientItem.DianResolution.Prefix,
                        number = invoice,
                        orderReference = new OrderReferenceEntity
                        {
                            id_order = "COT2022043155"
                        },
                        send_email = true,
                        attachment1 = new AttachmentEntity
                        {
                            filename = "prueba.xml",
                            b64data = "-> lugar para el archivo convertido a base64 string"
                        },
                        attachment2 = new AttachmentEntity
                        {
                            filename = "prueba.xml",
                            b64data = "-> lugar para el archivo convertido a base64 string"
                        },

                        customer = new CustomerEntity
                        {
                            identification_number = identification,
                            dv = dv,
                            name = item.contact_name,
                            phone = item.mobile,
                            address = "Cra 4ta #12-56",
                            email = item.email,
                            merchant_registration = "00000000",
                            type_document_identification_id = (int)documentType,
                            type_organization_id = 1,
                            type_liability_id = 117,
                            municipality_id = 149,
                            type_regime_id = 1
                        },
                        payment = new PaymentEntity
                        {
                            payment_form_id = 1,
                            payment_method_id = 10,
                            payment_due_date = formattedDate,
                            duration_measure = "30"
                        },
                        generalAllowances = [],
                        items = Itens,
                        resolution = clientItem.DianResolution.ResolutionNumber,
                        resolutionText = clientItem.DianResolution.Description,
                        head_note = item.invoice,
                        foot_note = item.invoice,
                        notes = $"Fecha de la factura:{item.transaction_date}",
                        allowanceTotal = 0,
                        invoiceBaseTotal = totalToBase,
                        invoiceTaxExclusiveTotal = totalToBase,
                        invoiceTaxInclusiveTotal = totalToPay,
                        totalToPay = totalToPay,
                        allTaxTotals = allTaxTotals,
                        allHoldingsTaxTotals = [
                    new AllHoldingsTaxTotalEntity
                        {
                            tax_id = 6,
                            tax_amount = 0,
                            percent = 0,
                            taxable_amount = totalToPay
                        }],

                        customSubtotals = [],
                        finalTotalToPay = totalToPay
                    };
                    var jsonContent = JsonConvert.SerializeObject(data);
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    using var client = new HttpClient();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientItem.ApiKey);
                    var request = new HttpRequestMessage(HttpMethod.Post, bool.Parse(config["Enviroment:Production"]!) ? config["ApiPlemsi:FEUrl"] : config["ApiPlemsiQa:FEUrl"])
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
                           new ParametersCurrentlyNumber(invoice, item.transaction_date, clientItem.ResolutionId),
                           cancellationToken);


                    }
                    if (totalToPay > 0)
                    {
                        HttpResponseMessage response = !expirated ? await client.SendAsync(request) : new HttpResponseMessage();

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            var responseApi = JsonConvert.DeserializeObject<ApiResponseFERetailPos>(jsonResponse);
                            if (responseApi is not null && responseApi.Success)
                            {
                                await insertInvoice.InsertInvoiceSucces(
                                    invoice,
                                    responseApi.Data.Cude,
                                    responseApi.Data.QRCode,
                                    connectionString,
                                    clientItem.ProviderId,
                                    clientItem.ClientBillingElectronicId,
                                    item.invoice);

                                await updateCurrentlyNumber.UpdateCurrentlyNumberAsync(
                                    new ParametersCurrentlyNumber(invoice, item.transaction_date, clientItem.ResolutionId),
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
            
        }
        return FERetailResponse;

    }

    private DocumentType ValidarGuion(string input)
    {
        return input.Contains('-') ? DocumentType.NIT : DocumentType.CC;
    }

    private static List<AllTaxTotalEntity> GetAllTaxTotals(IEnumerable<ItemElectronicEntity> items)
    {
        var allTaxTotals = new List<AllTaxTotalEntity>();

        foreach (var item in items)
        {
            foreach (var tax in item.tax_totals)
            {
                allTaxTotals.Add(new AllTaxTotalEntity
                {
                    tax_id = tax.tax_id,
                    tax_amount = tax.tax_amount,
                    percent = tax.percent,
                    taxable_amount = tax.taxable_amount
                });
            }
        }

        return allTaxTotals;
    }

}