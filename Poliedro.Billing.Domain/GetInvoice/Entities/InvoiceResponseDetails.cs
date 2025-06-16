namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class InvoiceResponseDetails
{
    public string Message { get; set; }
    public InvoiceErrorData Data { get; set; }
}
