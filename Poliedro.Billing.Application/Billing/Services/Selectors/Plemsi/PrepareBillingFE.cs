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
    IAllowanceChargesBilling _allowanceChargesBilling,
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

        bool expirated = lastInvoiceNumber + invoices.Count() > clientInfo.FinalRange || DateTime.Now > clientInfo.ExpirationDate;
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

                // Procesar ítems
                invoice.ItemElectronicEntity = await _prepareItemElectronic.PrepareItemBillingAsync(invoice.ItemElectronicEntity);

                // Calcular totales reales
                double totalBruto = invoice.ItemElectronicEntity?.Sum(i => i.PriceAmount * i.InvoicedQuantity) ?? 0; // base sin descuento
                double totalDescuentos = invoice.ItemElectronicEntity?.Sum(i => i.LineDiscountAmount) ?? 0;
                double totalImpuestos = invoice.ItemElectronicEntity?.Sum(i => i.TaxTotals?.Sum(t => t.TaxAmount) ?? 0) ?? 0;

                // Agregar descuento global si aplica
                if (invoice.DiscountAmountByInvoice > 0)
                {
                    totalDescuentos += (double)invoice.DiscountAmountByInvoice;
                }

                // Base después del descuento
                double baseNeta = totalBruto - totalDescuentos;

                // Total a pagar (Regla FAU14 + coherencia FAU08)
                double totalPagar = baseNeta + totalImpuestos;

                if (totalPagar <= 0)
                {
                    Console.WriteLine($"Factura {invoice.Number}: total a pagar {totalPagar} inválido.");
                    continue;
                }

                // Asignar coherentemente los totales
                invoice.InvoiceBaseTotal = totalBruto;                      // Total antes de descuento
                invoice.AllowanceTotal = totalDescuentos;                   // Total descuentos
                invoice.InvoiceTaxExclusiveTotal = totalBruto;                // Base neta (antes de impuestos)
                invoice.InvoiceTaxInclusiveTotal = totalBruto + totalImpuestos;
                invoice.TotalBeforeTax = (decimal?)baseNeta;
                invoice.TotalToPay = totalPagar;
                invoice.FinalTotalToPay = totalPagar;

                //Calcular descuentos
                double DiscountPercent = (totalDescuentos / totalBruto) * 100;
                double RoundedDiscountPercent =  Math.Round(DiscountPercent,2);


                // Calcular impuestos totales
                invoice.AllTaxTotalEntity = await _getAllTaxTotals.IGetAllTaxTotalsBillingAsync(invoice.ItemElectronicEntity);

                // Generar número de factura
                int invoiceNumber = invoiceCounter++;
                string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");

                invoice.Prefix = clientInfo.Prefix;
                invoice.CustomerEntity.ApiKey = clientInfo.ApiKey;
                invoice.Numeration = invoiceNumber.ToString();

                // Validar tipo de documento y dígito de verificación
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

                // Construcción de la entidad electrónica
                FERetailelectronicEntity data = new()
                {
                    date = formattedDate,
                    time = currentTime,
                    prefix = invoice.Prefix,
                    number = invoiceNumber,

                    orderReference = new OrderReferenceEntity { IdOrder = "COT2022043155" },
                    send_email = true,
                    attachment1 = new AttachmentEntity { FileName = "prueba.xml", B64Data = "-> lugar para el archivo convertido a base64 string" },
                    attachment2 = new AttachmentEntity { FileName = "prueba.xml", B64Data = "-> lugar para el archivo convertido a base64 string" },

                    customer = new CustomerEntity
                    {
                        IdentificationNumber = identification,
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

                    generalAllowances = [
                        new GeneralAllowanceEntity
                        {
                            AllowanceChargeReason = "Descuento Comercial",
                            AllowancePercent = RoundedDiscountPercent,
                            Amount = (decimal)totalDescuentos,
                            BaseAmount = (decimal)totalBruto
                        }
                        ],

                    items = invoice.ItemElectronicEntity,
                    resolution = clientInfo.ResolucionNumber,
                    resolutionText = clientInfo.Descripcion,
                    head_note = invoice.Number,
                    foot_note = invoice.Number,
                    notes = $"Fecha de la factura:{invoice.TransactionDate}",

               

                    allowanceTotal = totalDescuentos,               // 👈 Total descuentos reales
                    invoiceBaseTotal = totalBruto,                  // Total antes de descuentos
                    invoiceTaxExclusiveTotal = invoice.InvoiceTaxExclusiveTotal,            
                    invoiceTaxInclusiveTotal = invoice.InvoiceTaxInclusiveTotal,
                    totalToPay = totalPagar,
                   

                    allTaxTotals = invoice.AllTaxTotalEntity,

                    allHoldingsTaxTotals = [ 
                        new AllHoldingsTaxTotalEntity
                        {
                            TaxId = 6,
                            TaxAmount = 0,
                            Percent = 0,
                            TaxableAmount = totalPagar
                        }
                    ],
                    customSubtotals = [],
                    finalTotalToPay = totalPagar
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