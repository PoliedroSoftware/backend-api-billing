namespace Poliedro.Billing.Domain.Billing;

public class ItemsInvoiceEntity
{
    public string Resolution { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Code { get; set; } = default!;
    public double BaseQuantity { get; set; } = default!;
    public double InvoicedQuantity { get; set; } = default!;
    public decimal PriceAmount { get; set; } = default!;
    public double Subtotal { get; set; } = default!;
}
