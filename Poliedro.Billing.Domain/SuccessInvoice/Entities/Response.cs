using Poliedro.Billing.Domain.InvoicePos.Entities;

namespace Poliedro.Billing.Domain.SuccessInvoice.Entities;

public class Response
{
    public string? Message { get; set; }
    public ResponseData? Data { get; set; }
}
