namespace Poliedro.Billing.Application.Billing.Ports;
public class BillingResultItem
{
    public string? InvoiceNumber { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }

}
