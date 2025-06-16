namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class Response
{
    public string message { get; set; } = default!;
    public ResponseData data { get; set; } = default!;
}
