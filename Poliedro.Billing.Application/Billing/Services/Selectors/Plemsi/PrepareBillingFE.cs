using AutoMapper;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingFE(
    IPrepareItemBilling _prepareItemElectronic,
    IGetAllTaxTotalsBilling _getAllTaxTotals,
    IGetLastInvoiceBilling _getLastInvoiceBilling,
    IBillingValidateScript _billingValidateScript,
    ICalculateCheckDigits _calculateCheckDigits,
    IMapper _mapper,
    IConfiguration _config
    ) : ICreateBilling 
{
    public async Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(
        IEnumerable<CreateBilling> invoices, BillingInfoClient clientInfo, CancellationToken cancellationToken)
    {

        var results = new List<(CreateBilling Billing, object Output)>();

        // Obtener el ultimo numero
        int lastInvoiceNumber = await _getLastInvoiceBilling.GetLastInvoiceNumberAsync(clientInfo, cancellationToken);

        
        bool expirated = lastInvoiceNumber + invoices.Count() > clientInfo.FinalRange || DateTime.Now > clientInfo.ExpirationDate;
        if (expirated)
        {
            throw new Exception("El rango de numeración ha sido superado o la resolución ha expirado.");
        }

        // Inicializar contador de facturas
        int invoiceCounter = lastInvoiceNumber;

        foreach (var invoice in invoices)
        {
            // Calcular totales de items
            if (invoice.ItemElectronicEntity != null)
            {
                invoice.ItemElectronicEntity = await _prepareItemElectronic.PrepareItemBillingAsync(invoice.ItemElectronicEntity);

                double totalToBase = invoice.ItemElectronicEntity?.Sum(item => item.LineExtensionAmount) ?? 0;
                double totalTaxableAmount = invoice.ItemElectronicEntity?.Sum(item => item.TaxTotals?.Sum(tax => tax.TaxAmount)) ?? 0;
                double totalBaseGravable = invoice.ItemElectronicEntity?.Sum(item => item.TaxTotals?.Sum(tax => tax.TaxableAmount)) ?? 0;
                double totalToPay = totalToBase + totalTaxableAmount;

                invoice.InvoiceBaseTotal = (double)totalToBase;
                invoice.InvoiceTaxExclusiveTotal = (double)totalToBase;
                invoice.InvoiceTaxInclusiveTotal = (double)totalToPay;
                invoice.TotalToPay = (double)totalToPay;
                invoice.FinalTotalToPay = (double)totalToPay;

                List<AllTaxTotalEntity> AllTaxTotals = [];

                if (invoice.ItemElectronicEntity == null) continue;
               
                invoice.AllTaxTotalEntity = await _getAllTaxTotals.IGetAllTaxTotalsBillingAsync(invoice.ItemElectronicEntity);

            }
           
            invoice.Prefix = clientInfo.Prefix;
            invoice.CustomerEntity.ApiKey = clientInfo.ApiKey;
            // Usar el contador local
            int InvoiceNumber = invoiceCounter++;

            // Fecha y hora
            DateTime Date = DateTime.Now;
            string FormattedDate = Date.ToString("yyyy-MM-dd");
            string CurrentTime = Date.ToString("HH:mm:ss");

            // Validación de documento
            DocumentType DocumentType = await _billingValidateScript.ValidateScriptAsync(invoice.CustomerEntity.IdentificationNumber, cancellationToken);
            if (DocumentType == DocumentType.NIT)
            {
                invoice.CustomerEntity.IdentificationNumber = invoice.CustomerEntity.IdentificationNumber.Replace("-", "");
                if (invoice.CustomerEntity.IdentificationNumber.Length > 0)
                {
                    invoice.CustomerEntity.IdentificationNumber = invoice.CustomerEntity.IdentificationNumber.Substring(0, invoice.CustomerEntity.IdentificationNumber.Length - 1);
                }
            }

            // Calcular dígito de verificación
            string Identification = invoice.CustomerEntity.IdentificationNumber.Trim().Replace(".", "").Replace("-", "").Replace(" ", "").Replace("+", "");
            string checkDigit = await _calculateCheckDigits.CalculateCheckDigit(Identification, cancellationToken);
            if (checkDigit == "error")
            {
                Identification = _config["CosumerFinal:identification"];
                checkDigit = _config["CosumerFinal:dv"];
            }

            // Validar Total
            if (invoice.FinalTotalToPay > 0)
            {
                // Crear el objeto final
                FERetailelectronicEntity Data = new()
                {
                    date = FormattedDate,
                    time = CurrentTime,
                    prefix = invoice.Prefix,
                    number = InvoiceNumber,

                    orderReference = new OrderReferenceEntity
                    {
                        IdOrder = "COT2022043155"
                    },

                    send_email = true,
                    attachment1 = new AttachmentEntity
                    {
                        FileName = "prueba.xml",
                        B64Data = "-> lugar para el archivo convertido a base64 string"
                    },
                    attachment2 = new AttachmentEntity
                    {
                        FileName = "prueba.xml",
                        B64Data = "-> lugar para el archivo convertido a base64 string"
                    },

                    customer = new CustomerEntity
                    {
                        IdentificationNumber = Identification,
                        Dv = checkDigit,
                        Name = invoice.CustomerEntity.Name,
                        Phone = invoice.CustomerEntity.Phone,
                        Address = "Cra 4ta #12-56",
                        Email = invoice.CustomerEntity.Email,
                        MerchantRegistration = "00000000",
                        MunicipalityCode = "11001",
                        TypeDocumentIdentificationId = (int)DocumentType,
                        TypeOrganizationId = 1,
                        TypeLiabilityId = 117,
                        MunicipalityId = 149,
                        TypeRegimeId = 1
                    },

                    payment = new PaymentEntity
                    {
                        PaymentFormId = 1,
                        PaymentMethodId = 10,
                        PaymentDueDate = FormattedDate,
                        DurationMeasure = "30"
                    },

                    generalAllowances = [],
                    items = invoice.ItemElectronicEntity,
                    resolution = clientInfo.ResolucionNumber,
                    resolutionText = clientInfo.Descripcion,
                    head_note = invoice.Number,
                    foot_note = invoice.Number,
                    notes = $"Fecha de la factura:{invoice.TransactionDate}",
                    allowanceTotal = 0,
                    invoiceBaseTotal = invoice.InvoiceBaseTotal,
                    invoiceTaxExclusiveTotal = invoice.InvoiceTaxExclusiveTotal,
                    invoiceTaxInclusiveTotal = invoice.InvoiceTaxInclusiveTotal,
                    totalToPay = invoice.TotalToPay,
                    allTaxTotals = invoice.AllTaxTotalEntity,
                    allHoldingsTaxTotals = [
                    new AllHoldingsTaxTotalEntity
                        {
                            TaxId = 6,
                            TaxAmount = 0,
                            Percent = 0,
                            TaxableAmount = invoice.TotalToPay
                        }],

                    customSubtotals = [],
                    finalTotalToPay = invoice.TotalToPay


                };

                var dto = _mapper.Map<SenderRequestDTO>(Data); 

                results.Add((invoice, dto));

            }
            else
            {
                throw new Exception("El total a pagar no puede ser cero o negativo.");
            }
        }

        return results;

    }
}