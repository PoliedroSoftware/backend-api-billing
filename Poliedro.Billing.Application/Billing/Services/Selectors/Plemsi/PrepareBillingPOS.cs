using AutoMapper;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi.POS;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Billing.Pos.Entity;
using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
public class PrepareBillingPOS(
    IGetLastInvoiceBilling _getLastInvoiceBilling,
    IMapper _mapper
    ) : ICreateBilling
{
    public async Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, BillingInfoClient clientInfo, CancellationToken cancellationToken)
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

                if (invoice.TotalToPay <= 0)
                {
                    Console.WriteLine($"Factura {invoice.Number}: total a pagar {invoice.TotalToPay} invalido.");
                    continue;
                }

                if (invoice.ItemElectronicEntity == null) continue;

                int InvoiceNumber = int.Parse(invoice.Number[^6..]);
                string FormattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                List<ItemFERetailEntity> itemsInvoiceResponse = new();

                foreach (ItemElectronicEntity item in invoice.ItemElectronicEntity)
                {
                    var itemInvoice = new ItemFERetailEntity
                    {
                        unit_measure_id = 70,
                        invoiced_quantity = item.InvoicedQuantity.ToString(),
                        line_extension_amount = item.Subtotal.ToString(),
                        free_of_charge_indicator = false,
                        tax_totals =[],
                        description = item.Description,
                        notes = "xxxx",
                        code = item.Code.ToString(),
                        type_item_identification_id = 4,
                        price_amount = item.PriceAmount.ToString(),
                        base_quantity = item.InvoicedQuantity.ToString()
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
                    number = invoiceCounter,
                    date = FormattedDate,
                    time = CurrentTime,
                    softwareManufacturer = new SoftwareManufacturerEntity
                    {
                        ownerName = "Maicol Said Arevalo Gallardo",
                        softwareName = "Poliedro Pos",
                        companyName = "Poliedro Software S.A.S"
                    },
                    sendToEmail = "poliedrosoftware@gmail.com",
                    resolution = clientInfo.ResolucionNumber,
                    prefix = clientInfo.Prefix,
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
                var dto = _mapper.Map<InvoiceRequestPosDto>(Data);

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