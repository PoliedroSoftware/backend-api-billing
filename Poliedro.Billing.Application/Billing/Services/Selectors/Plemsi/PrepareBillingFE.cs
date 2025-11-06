using AutoMapper;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi
{
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
                Console.WriteLine($"Error al obtener último número de factura: {ex.Message}");
                return results;
            }

            bool expirated = lastInvoiceNumber + invoices.Count() > clientInfo.FinalRange ||
                             DateTime.Now > clientInfo.ExpirationDate;
            if (expirated)
            {
                Console.WriteLine("El rango de numeración ha sido superado o la resolución ha expirado.");
                return results;
            }

            int invoiceCounter = lastInvoiceNumber;

            foreach (var invoice in invoices)
            {
                try
                {
                    if (invoice.ItemElectronicEntity == null || !invoice.ItemElectronicEntity.Any())
                    {
                        Console.WriteLine($"Factura {invoice.Number}: sin items.");
                        continue;
                    }


                    invoice.ItemElectronicEntity = await _prepareItemElectronic.PrepareItemBillingAsync(
                        invoice.ItemElectronicEntity);


                    foreach (var item in invoice.ItemElectronicEntity)
                    {
                        if (item.AllowanceCharges != null)
                        {
                            foreach (var allowance in item.AllowanceCharges)
                            {
                                allowance.MultiplierFactorNumeric = Math.Round(
                                    allowance.MultiplierFactorNumeric, 6, MidpointRounding.AwayFromZero);
                            }
                        }
                    }




                    double globalDiscount = invoice.DiscountAmountByInvoice > 0 ? (double)invoice.DiscountAmountByInvoice : 0;


                    double invoiceBaseTotal = invoice.ItemElectronicEntity.Sum(i => (double)i.LineExtensionAmount);


                    double allowanceTotal = 0;

                    double invoiceTaxExclusiveTotal = invoiceBaseTotal;


                    double totalTaxes = Math.Round(invoice.ItemElectronicEntity.Sum(i => (double)(i.TaxTotals?.Sum(t => (double)t.TaxAmount) ?? 0.0)), 2, MidpointRounding.AwayFromZero);


                    double invoiceTaxInclusiveTotal = Math.Round(invoiceBaseTotal + totalTaxes, 2, MidpointRounding.AwayFromZero);


                    if (globalDiscount > 0)
                    {
                        allowanceTotal += Math.Round(globalDiscount, 2);
                    }


                    invoiceBaseTotal = Math.Round(invoiceBaseTotal, 2, MidpointRounding.AwayFromZero);


                    double totalToPay = invoiceTaxInclusiveTotal - Math.Round(globalDiscount, 2);

                    allowanceTotal = Math.Round(allowanceTotal, 2, MidpointRounding.AwayFromZero);

                    if (totalToPay <= 0)
                    {
                        Console.WriteLine($"Factura {invoice.Number}: total a pagar {totalToPay} inválido.");
                        continue;
                    }


                    invoice.InvoiceBaseTotal = invoiceBaseTotal;
                    invoice.AllowanceTotal = allowanceTotal;
                    invoice.InvoiceTaxExclusiveTotal = invoiceTaxExclusiveTotal;
                    invoice.InvoiceTaxInclusiveTotal = invoiceTaxInclusiveTotal;
                    invoice.TotalToPay = totalToPay;
                    invoice.FinalTotalToPay = totalToPay;


                    int invoiceNumber = invoiceCounter++;
                    string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");

                    invoice.AllTaxTotalEntity = await _getAllTaxTotals
                        .IGetAllTaxTotalsBillingAsync(invoice.ItemElectronicEntity);
                    invoice.Prefix = clientInfo.Prefix;
                    invoice.CustomerEntity.ApiKey = clientInfo.ApiKey;
                    invoice.Numeration = invoiceNumber.ToString();

                    


                    DocumentType documentType = await _billingValidateScript
                        .ValidateScriptAsync(invoice.CustomerEntity.IdentificationNumber, cancellationToken);


                    string identification = invoice.CustomerEntity.IdentificationNumber.Trim().Replace(".", "").Replace("-", "").Replace(" ", "").Replace("+", "");

                    invoice.CustomerEntity.IdentificationNumber = identification;

                    if (documentType == DocumentType.NIT)
                    {
                        invoice.CustomerEntity.IdentificationNumber =
                            invoice.CustomerEntity.IdentificationNumber.Replace("-", "");
                        if (invoice.CustomerEntity.IdentificationNumber.Length > 0)
                        {
                            invoice.CustomerEntity.IdentificationNumber = invoice.CustomerEntity.IdentificationNumber
                                .Substring(0, invoice.CustomerEntity.IdentificationNumber.Length - 1);
                        }
                    }


                    string checkDigit = await _calculateCheckDigits
                        .CalculateCheckDigit(invoice.CustomerEntity.IdentificationNumber, cancellationToken);

                    
                    if (checkDigit == "error")
                    {
                        identification = _config["CosumerFinal:identification"];
                        checkDigit = _config["CosumerFinal:dv"];
                    }


                    double discountPercent = invoiceBaseTotal > 0 && globalDiscount > 0
                        ? (globalDiscount / invoiceBaseTotal) * 100
                        : 0;
                    double roundedDiscountPercent = Math.Round(discountPercent, 2);


                    FERetailelectronicEntity data = new()
                    {
                        date = formattedDate,
                        time = currentTime,
                        prefix = invoice.Prefix,
                        number = invoiceNumber,
                        orderReference = new OrderReferenceEntity { IdOrder = "COT2022043155" },
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
                            IdentificationNumber = invoice.CustomerEntity.IdentificationNumber,
                            Dv = checkDigit,
                            Name = invoice.CustomerEntity.Name,
                            Phone = invoice.CustomerEntity.Phone,
                            Address = "Cra 4ta #12-56",
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
                            ? new List<GeneralAllowanceEntity> {
                                new GeneralAllowanceEntity {
                                    AllowanceChargeReason = "Commercial Discount",
                                    AllowancePercent = roundedDiscountPercent,
                                    Amount = Math.Round((decimal)globalDiscount, 2),
                                    BaseAmount = (decimal)invoiceBaseTotal
                                }
                            }
                            : new List<GeneralAllowanceEntity>(),

                        items = invoice.ItemElectronicEntity,
                        resolution = clientInfo.ResolucionNumber,
                        resolutionText = clientInfo.Descripcion,
                        head_note = string.IsNullOrWhiteSpace(clientInfo.HeadNote) ? invoice.Number : clientInfo.HeadNote,
                        foot_note = string.IsNullOrWhiteSpace(clientInfo.FootNote) ? invoice.Number : clientInfo.FootNote,
                        notes = $"Fecha de la factura:{invoice.TransactionDate}",


                        allowanceTotal = allowanceTotal,
                        invoiceBaseTotal = invoiceBaseTotal,
                        invoiceTaxExclusiveTotal = Math.Round(invoiceTaxExclusiveTotal, 1),
                        invoiceTaxInclusiveTotal = invoiceTaxInclusiveTotal,
                        totalToPay = totalToPay,

                        allTaxTotals = invoice.AllTaxTotalEntity,
                        allHoldingsTaxTotals = new List<AllHoldingsTaxTotalEntity> {
                            new AllHoldingsTaxTotalEntity {
                                TaxId = 6,
                                TaxAmount = 0,
                                Percent = 0,
                                TaxableAmount = totalToPay
                            }
                        },
                        customSubtotals = [],
                        finalTotalToPay = totalToPay
                    };

                    var dto = _mapper.Map<SenderRequestFEDTO>(data);
                    results.Add((invoice, dto));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando factura {invoice.Number}: {ex.Message}");
                }
            }

            return results;
        }
    }
}
