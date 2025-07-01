namespace Poliedro.Billing.Domain.BillingPos;

public class ItemsInvoiceEntity
{
    public string Resolution { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Code { get; set; } = default!;
    public decimal BaseQuantity { get; set; } = default!;
    public decimal InvoicedQuantity { get; set; } = default!;
    public decimal PriceAmount { get; set; } = default!;
    public decimal Subtotal { get; set; } = default!;
}
