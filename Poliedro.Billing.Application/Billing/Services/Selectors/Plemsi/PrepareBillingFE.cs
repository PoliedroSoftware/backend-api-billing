using AutoMapper;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi
{
    public class PrepareBillingFE(
        IPrepareItemBilling _prepareItemElectronic,
        IGetAllTaxTotalsBilling _getAllTaxTotals,
        IGetLastInvoiceBilling _getLastInvoiceBilling,
        ICalculateCheckDigits _calculateCheckDigits,
        IMapper _mapper,
        IConfiguration _config
    ) : ICreateBilling
    {
        public async Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(
            IEnumerable<CreateBilling> invoices,
            DianResolutionEntity dianResolutionEntity,
            CompanyProviderEntity companyProviderEntity,
            CancellationToken cancellationToken)
        {
            var results = new List<(CreateBilling Billing, object Output)>();
            int lastInvoiceNumber;

            try
            {
                lastInvoiceNumber = await _getLastInvoiceBilling.GetLastInvoiceNumberAsync(dianResolutionEntity,companyProviderEntity, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener último número de factura: {ex.Message}");
                return results;
            }

            bool expirated = lastInvoiceNumber + invoices.Count() > dianResolutionEntity.FinalRange ||
                             DateTime.Now > dianResolutionEntity.ExpirationDate;
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

                    if (invoice.CustomerEntity?.TypeOfPerson == null || invoice.CustomerEntity.TypeOfPerson == 0)
                    {
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
                    invoice.Prefix = dianResolutionEntity.Prefix;
                    invoice.CustomerEntity.ApiKey = companyProviderEntity.ApiKey;
                    invoice.Numeration = invoiceNumber.ToString();


                  


                    string identification = invoice.CustomerEntity.IdentificationNumber.Trim().Replace(".", "").Replace("-", "").Replace(" ", "").Replace("+", "");

                    invoice.CustomerEntity.IdentificationNumber = identification;


                    string checkDigit = await _calculateCheckDigits
                        .CalculateCheckDigit(invoice.CustomerEntity.IdentificationNumber, cancellationToken);

                    
                    if (checkDigit == "error")
                    {
                        invoice.CustomerEntity.IdentificationNumber = _config["CosumerFinal:identification"];
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
                            Address = invoice.CustomerEntity.Address,
                            Email = invoice.CustomerEntity.Email,
                            MerchantRegistration = "00000000",
                            MunicipalityCode = invoice.CustomerEntity.MunicipalityCode,
                            TypeDocumentIdentificationId = invoice.CustomerEntity.TypeOfPerson,
                            TypeOrganizationId = 1,
                            TypeLiabilityId = 117,
                            MunicipalityId = invoice.CustomerEntity.MunicipalityId,
                            TypeRegimeId = 1
                        },
                        payment = new PaymentEntity
                        {
                            PaymentFormId = invoice.PaymentEntity.PaymentFormId,
                            PaymentMethodId = 10,
                            PaymentDueDate = formattedDate,
                            DurationMeasure = "30"
                        },


                        generalAllowances = globalDiscount > 0
                        ? new List<GeneralAllowanceEntity>
                        {
                            new GeneralAllowanceEntity
                            {
                                AllowanceChargeReason = "Commercial Discount",
                                AllowancePercent = roundedDiscountPercent,
                                MultiplierFactorNumeric = roundedDiscountPercent > 0 ? 1 : 0,
                                Amount = Math.Round((decimal)globalDiscount, 2),
                                BaseAmount = (decimal)invoiceBaseTotal
                            }
                        }
                        : new List<GeneralAllowanceEntity>(),


                        items = invoice.ItemElectronicEntity,
                        resolution = dianResolutionEntity.ResolutionNumber,
                        resolutionText = dianResolutionEntity.Description,
                        head_note = string.IsNullOrWhiteSpace(companyProviderEntity.HeadNote) ? invoice.Number : companyProviderEntity.HeadNote,
                        foot_note = string.IsNullOrWhiteSpace(companyProviderEntity.FooterNote) ? invoice.Number : companyProviderEntity.FooterNote,
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
