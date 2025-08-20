using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
public class PrepareBillingPOS(
    IGetLastInvoiceBilling _getLastInvoiceBilling
    ) : ICreateBilling //POS
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

                int InvoiceNumber = int.Parse(invoice.Number[^6..]);
                string FormattedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

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
                        Code = "000001",
                        Address = "Direccion Principal",
                        CashierName = "Cajero de Turno",
                        PayPointType = "Caja Auxiliar",
                        SaleCode = "V2398123",
                    }



                };




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando factura {invoice.Number}: {ex.Message}");
            }
        }
        return results;



    }
}