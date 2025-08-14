namespace Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
public class PlemsiInvoiceRequest
{
    public required string ApiKey { get; init; }
    public required IEnumerable<object> Invoices { get; init; }
}
