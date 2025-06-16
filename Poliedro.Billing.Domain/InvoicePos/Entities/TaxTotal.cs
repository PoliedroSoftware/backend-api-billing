namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class TaxTotal
{
    public int tax_id { get; set; }
    public string tax_amount { get; set; } = string.Empty;
    public string taxable_amount { get; set; } = string.Empty;
    public decimal percent { get; set; }
}
