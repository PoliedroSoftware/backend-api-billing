using AutoMapper;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

                    // Procesar items (devuelven LineExtensionAmount ya con descuentos y TaxTotals con montos redondeados)
                    invoice.ItemElectronicEntity = await _prepareItemElectronic.PrepareItemBillingAsync(
                        invoice.ItemElectronicEntity);

                    // =============== CÁLCULOS SEGÚN DIAN ===============

                    // Si existe un descuento global (campo original en invoice)
                    double globalDiscount = invoice.DiscountAmountByInvoice > 0 ? (double)invoice.DiscountAmountByInvoice : 0;

                    // Base = suma de LineExtensionAmount (ya con descuentos aplicados por línea)
                    double invoiceBaseTotal = invoice.ItemElectronicEntity.Sum(i => (double)i.LineExtensionAmount);

                    // Total descuentos: suma de montos redondeados en AllowanceCharges (informativo)
                    double allowanceTotal = 0;

                    double invoiceTaxExclusiveTotal = invoiceBaseTotal;

                    // Impuestos: suma de TaxAmount por línea (ya redondeados en PrepareItemElectronic)
                    double totalTaxes = Math.Round(invoice.ItemElectronicEntity.Sum(i => (double)(i.TaxTotals?.Sum(t => (double)t.TaxAmount) ?? 0.0)), 2, MidpointRounding.AwayFromZero);

                    // invoiceTaxInclusiveTotal = base + impuestos
                    double invoiceTaxInclusiveTotal = Math.Round(invoiceBaseTotal + totalTaxes, 2, MidpointRounding.AwayFromZero);

                    // Incluir descuento global (si existe) — también redondeado
                    if (globalDiscount > 0)
                    {
                        allowanceTotal += Math.Round(globalDiscount, 2);
                        
                    }

                    // Redondear invoiceBaseTotal (asegurar 2 decimales)
                    invoiceBaseTotal = Math.Round(invoiceBaseTotal, 2, MidpointRounding.AwayFromZero);

                    // totalToPay = invoiceTaxInclusiveTotal (si no hay cargos adicionales)
                    double totalToPay = invoiceTaxInclusiveTotal - Math.Round(globalDiscount,2);

                    // Asegurar que allowanceTotal también esté con 2 decimales
                    allowanceTotal = Math.Round(allowanceTotal, 2, MidpointRounding.AwayFromZero);

                    if (totalToPay <= 0)
                    {
                        Console.WriteLine($"Factura {invoice.Number}: total a pagar {totalToPay} inválido.");
                        continue;
                    }

                    // Asignar valores al objeto invoice (redondeados)
                    invoice.InvoiceBaseTotal = invoiceBaseTotal;
                    invoice.AllowanceTotal = allowanceTotal;
                    invoice.InvoiceTaxExclusiveTotal = invoiceTaxExclusiveTotal;
                    invoice.InvoiceTaxInclusiveTotal = invoiceTaxInclusiveTotal;
                    invoice.TotalToPay = totalToPay;
                    invoice.FinalTotalToPay = totalToPay;

                    // Generar número de factura
                    int invoiceNumber = invoiceCounter++;
                    string formattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string currentTime = DateTime.Now.ToString("HH:mm:ss");

                    invoice.AllTaxTotalEntity = await _getAllTaxTotals
                        .IGetAllTaxTotalsBillingAsync(invoice.ItemElectronicEntity);
                    invoice.Prefix = clientInfo.Prefix;
                    invoice.CustomerEntity.ApiKey = clientInfo.ApiKey;
                    invoice.Numeration = invoiceNumber.ToString();

                    // Validar tipo de documento y dígito de verificación
                    DocumentType documentType = await _billingValidateScript
                        .ValidateScriptAsync(invoice.CustomerEntity.IdentificationNumber, cancellationToken);

                    string identification = invoice.CustomerEntity.IdentificationNumber
                        .Trim().Replace(".", "").Replace("-", "").Replace(" ", "").Replace("+", "");
                    string checkDigit = await _calculateCheckDigits
                        .CalculateCheckDigit(identification, cancellationToken);

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

                    if (checkDigit == "error")
                    {
                        identification = _config["CosumerFinal:identification"];
                        checkDigit = _config["CosumerFinal:dv"];
                    }

                    // Calcular porcentaje de descuento global (si aplica)
                    double discountPercent = invoiceBaseTotal > 0 && globalDiscount > 0
                        ? (globalDiscount / invoiceBaseTotal) * 100
                        : 0;
                    double roundedDiscountPercent = Math.Round(discountPercent, 2);

                    // Construir entidad electrónica (usamos los valores ya redondeados)
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

                        // Incluir descuento global solo si existe (ya redondeado)
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
                        head_note = invoice.Number,
                        foot_note = invoice.Number,
                        notes = $"Fecha de la factura:{invoice.TransactionDate}",

                        // Totales según validación DIAN (ya redondeados)
                        allowanceTotal = allowanceTotal,
                        invoiceBaseTotal = invoiceBaseTotal,
                        invoiceTaxExclusiveTotal = Math.Round(invoiceTaxExclusiveTotal,1),
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
