using AutoMapper;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
using System.Text.Json.Serialization;

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
        int lastInvoiceNumber;

        try
        {
            lastInvoiceNumber = await _getLastInvoiceBilling.GetLastInvoiceNumberAsync(clientInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting last invoice number: {ex.Message}");
            return results;
        }

        bool isExpired = lastInvoiceNumber + invoices.Count() > clientInfo.FinalRange || DateTime.Now > clientInfo.ExpirationDate;
        if (isExpired)
        {
            Console.WriteLine("The numbering range has been exceeded or the resolution has expired.");
            return results;
        }

        int invoiceCounter = lastInvoiceNumber;

        foreach (var invoice in invoices)
        {
            try
            {
                if (invoice.ItemElectronicEntity == null || !invoice.ItemElectronicEntity.Any())
                {
                    Console.WriteLine($"Invoice {invoice.Number}: no items.");
                    continue;
                }

                // Process items
                invoice.ItemElectronicEntity = await _prepareItemElectronic.PrepareItemBillingAsync(invoice.ItemElectronicEntity);

                // ============ CÁLCULOS SEGÚN UBL 2.1 / DIAN ============

                // FAU02: invoiceBaseTotal = Suma de LineExtensionAmount (valor ANTES de descuentos)
                double invoiceBaseTotal = invoice.ItemElectronicEntity?.Sum(i => i.LineExtensionAmount) ?? 0;

                // FAU08: allowanceTotal = Suma de allowance_charges
                double totalLineDiscounts = invoice.ItemElectronicEntity?.Sum(i =>
                    i.AllowanceCharges?.Where(a => !a.ChargeIndicator).Sum(a => (double)a.Amount) ?? 0) ?? 0;

                // Impuestos (calculados sobre base después de descuento)
                double totalTaxes = invoice.ItemElectronicEntity?.Sum(i =>
                    i.TaxTotals?.Sum(t => t.TaxAmount) ?? 0) ?? 0;

                // Descuento global
                double globalDiscount = (double)(invoice.DiscountAmountByInvoice > 0
                    ? invoice.DiscountAmountByInvoice
                    : 0);

                // FAU08: Total de TODOS los descuentos
                double allowanceTotal = totalLineDiscounts + globalDiscount;

                // invoiceTaxExclusiveTotal: invoiceBaseTotal - allowanceTotal
                double invoiceTaxExclusiveTotal = invoiceBaseTotal - allowanceTotal;

                // FAU06: invoiceTaxInclusiveTotal = invoiceBaseTotal - allowanceTotal + impuestos
                double invoiceTaxInclusiveTotal = invoiceTaxExclusiveTotal + totalTaxes;

                // FAU14: totalToPay = invoiceBaseTotal + tributos - allowanceTotal + cargos
                double totalToPay = invoiceBaseTotal + totalTaxes - allowanceTotal;

                if (totalToPay <= 0)
                {
                    Console.WriteLine($"Invoice {invoice.Number}: invalid total to pay {totalToPay}.");
                    continue;
                }

                // Asignar valores
                invoice.InvoiceBaseTotal = invoiceBaseTotal;
                invoice.AllowanceTotal = allowanceTotal;
                invoice.InvoiceTaxExclusiveTotal = invoiceTaxExclusiveTotal;
                invoice.InvoiceTaxInclusiveTotal = invoiceTaxInclusiveTotal;
                invoice.TotalBeforeTax = (decimal?)invoiceTaxExclusiveTotal;
                invoice.TotalToPay = totalToPay;
                invoice.FinalTotalToPay = totalToPay;

                // Calculate discount percent for global discount
                double discountPercent = invoiceBaseTotal > 0 && globalDiscount > 0
                    ? (globalDiscount / invoiceBaseTotal) * 100
                    : 0;
                double roundedDiscountPercent = Math.Round(discountPercent, 2);

                // Calculate total taxes grouped
                invoice.AllTaxTotalEntity = await _getAllTaxTotals.IGetAllTaxTotalsBillingAsync(invoice.ItemElectronicEntity);

                // Generate invoice number
                int invoiceNumber = invoiceCounter++;
                string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");

                invoice.Prefix = clientInfo.Prefix;
                invoice.CustomerEntity.ApiKey = clientInfo.ApiKey;
                invoice.Numeration = invoiceNumber.ToString();

                // Validate document type and check digit
                DocumentType documentType = await _billingValidateScript.ValidateScriptAsync(invoice.CustomerEntity.IdentificationNumber, cancellationToken);
                string identification = invoice.CustomerEntity.IdentificationNumber.Trim().Replace(".", "").Replace("-", "").Replace(" ", "").Replace("+", "");
                string checkDigit = await _calculateCheckDigits.CalculateCheckDigit(identification, cancellationToken);

                if (documentType == DocumentType.NIT)
                {
                    invoice.CustomerEntity.IdentificationNumber = invoice.CustomerEntity.IdentificationNumber.Replace("-", "");
                    if (invoice.CustomerEntity.IdentificationNumber.Length > 0)
                    {
                        invoice.CustomerEntity.IdentificationNumber = invoice.CustomerEntity.IdentificationNumber.Substring(0, invoice.CustomerEntity.IdentificationNumber.Length - 1);
                    }
                }

                if (checkDigit == "error")
                {
                    identification = _config["CosumerFinal:identification"];
                    checkDigit = _config["CosumerFinal:dv"];
                }

                // Build electronic entity
                FERetailelectronicEntity data = new()
                {
                    date = formattedDate,
                    time = currentTime,
                    prefix = invoice.Prefix,
                    number = invoiceNumber,

                    orderReference = new OrderReferenceEntity { IdOrder = "COT2022043155" },
                    send_email = true,
                    attachment1 = new AttachmentEntity { FileName = "test.xml", B64Data = "-> placeholder for base64 string file" },
                    attachment2 = new AttachmentEntity { FileName = "test.xml", B64Data = "-> placeholder for base64 string file" },

                    customer = new CustomerEntity
                    {
                        IdentificationNumber = identification,
                        Dv = checkDigit,
                        Name = invoice.CustomerEntity.Name,
                        Phone = invoice.CustomerEntity.Phone,
                        Address = "Cra 4th #12-56",
                        Email = invoice.CustomerEntity.Email,
                        MerchantRegistration = "00000000",
                        MunicipalityCode = "11001",
                        TypeDocumentIdentificationId = (int)documentType,
                        TypeOrganizationId = 1,
                        TypeLiabilityId = 117,
                        MunicipalityId = 149,
                        TypeRegimeId = 1
                    },

                    payment = new PaymentEntity
                    {
                        PaymentFormId = 1,
                        PaymentMethodId = 10,
                        PaymentDueDate = formattedDate,
                        DurationMeasure = "30"
                    },

                    generalAllowances = globalDiscount > 0
                        ? [
                            new GeneralAllowanceEntity
                        {
                            AllowanceChargeReason = "Commercial Discount",
                            AllowancePercent = roundedDiscountPercent,
                            Amount = (decimal)globalDiscount,
                            BaseAmount = (decimal)invoiceBaseTotal
                        }
                        ]
                        : [],

                    items = invoice.ItemElectronicEntity,
                    resolution = clientInfo.ResolucionNumber,
                    resolutionText = clientInfo.Descripcion,
                    head_note = invoice.Number,
                    foot_note = invoice.Number,
                    notes = $"Invoice date: {invoice.TransactionDate}",

                    allowanceTotal = allowanceTotal,
                    invoiceBaseTotal = invoiceBaseTotal,
                    invoiceTaxExclusiveTotal = invoiceTaxExclusiveTotal,
                    invoiceTaxInclusiveTotal = invoiceTaxInclusiveTotal,
                    totalToPay = totalToPay,

                    allTaxTotals = invoice.AllTaxTotalEntity,

                    allHoldingsTaxTotals = [
                        new AllHoldingsTaxTotalEntity
                    {
                        TaxId = 6,
                        TaxAmount = 0,
                        Percent = 0,
                        TaxableAmount = totalToPay
                    }
                    ],
                    customSubtotals = [],
                    finalTotalToPay = totalToPay
                };

                var dto = _mapper.Map<SenderRequestFEDTO>(data);
                results.Add((invoice, dto));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing invoice {invoice.Number}: {ex.Message}.");
            }
        }

        return results;
    }
}