using AutoMapper;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Billing.Pos.Entity;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Application.Billing.Services.Preparation;
public class PrepareBillingPOS(
    IMapper _mapper
    ) : ICreateBilling
{
    public async Task<IEnumerable<BillingPreparationResult>> CreateInvoicesAsync(
        IEnumerable<CreateBilling> invoices,
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        int lastInvoiceNumber,
        CancellationToken cancellationToken)
    {

        var results = new List<BillingPreparationResult>();
        // Sequence is obtained through the provider-specific numbering port in the handler.

        bool expirated = lastInvoiceNumber + invoices.Count() > dianResolutionEntity.FinalRange || DateTime.Now > dianResolutionEntity.ExpirationDate;
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
                int InvoiceNumber = invoiceCounter++;
                invoice.Numeration = InvoiceNumber.ToString();
                invoice.Number = invoice.Resolution;
                invoice.Prefix = dianResolutionEntity.Prefix;
                if (invoice.CustomerEntity == null)
                {
                    invoice.CustomerEntity = new CustomerEntity();
                }
               



                string FormattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                List<ItemFERetailEntity> itemsInvoiceResponse = new();

                if (invoice.ItemElectronicEntity == null || !invoice.ItemElectronicEntity.Any())
                {
                    Console.WriteLine($"Factura {invoice.Number}: sin items.");
                    continue;
                }

                if (invoice.TotalToPay <= 0)
                {
                    Console.WriteLine($"Factura {invoice.Number}: total a pagar {invoice.TotalToPay} invalido.");
                    continue;
                }

                if (invoice.ItemElectronicEntity == null) continue;


                foreach (ItemElectronicEntity item in invoice.ItemElectronicEntity)
                {
                    var itemInvoice = new ItemFERetailEntity
                    {
                        unit_measure_id = 70,
                        invoiced_quantity = item.InvoicedQuantity,
                        line_extension_amount = item.Subtotal,
                        free_of_charge_indicator = false,
                        tax_totals =[],
                        description = item.Description,
                        notes = "xxxx",
                        code = item.Code.ToString(),
                        type_item_identification_id = 4,
                        price_amount = item.PriceAmount.ToString(),
                        base_quantity = item.InvoicedQuantity
                    };
                    itemsInvoiceResponse.Add(itemInvoice);
                }

                if(itemsInvoiceResponse.Count == 0)
                {
                    Console.WriteLine($"Factura {invoice.Number}: sin items procesados.");
                    continue;
                }

                InvoicePosEntity Data = new()
                {
                    number = InvoiceNumber,
                    date = FormattedDate,
                    time = CurrentTime,
                    softwareManufacturer = new SoftwareManufacturerEntity
                    {
                        ownerName = "Maicol Said Arevalo Gallardo",
                        softwareName = "Poliedro Pos",
                        companyName = "Poliedro Software S.A.S"
                    },
                    sendToEmail = "poliedrosoftware@gmail.com",
                    resolution = dianResolutionEntity.ResolutionNumber,
                    prefix = dianResolutionEntity.Prefix,
                    head_note = $"Fecha de la factura:{CurrentTime}",
                    foot_note = invoice.Number,
                    payment =  new PaymentPosEntity
                    {
                        payment_form_id = 1,
                        payment_method_id = 30,
                        payment_due_date = FormattedDate,
                        duration_measure = "1"
                    },
                    payPointInfo =  new PayPointInfoEntity
                    {
                        code = "000001",
                        address = "Direccion Principal",
                        cashierName = "Cajero de Turno",
                        payPointType = "Caja Auxiliar",
                        saleCode = "V2398123",
                    },
                    invoiceBaseTotal = invoice.InvoiceBaseTotal.ToString(),
                    invoiceTaxExclusiveTotal = invoice.InvoiceTaxExclusiveTotal.ToString(),
                    invoiceTaxInclusiveTotal = invoice.InvoiceTaxInclusiveTotal.ToString(),
                    totalToPay = invoice.TotalToPay.ToString(),
                    allTaxTotals = [],
                    items = itemsInvoiceResponse
                };

                results.Add(new(invoice, PreparedBillingDocument.FromPointOfSaleInvoice(Data), null));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando factura {invoice.Number}: {ex.Message}");
                results.Add(new(invoice, null, $"Factura {invoice.Number} no emitida: {ex.Message}"));
            }
        }
        return results;

    }
    
}
