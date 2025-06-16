namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class LegalMonetaryTotals
{
    public string line_extension_amount { get; set; } = default!;
    public string tax_exclusive_amount { get; set; } = default!;
    public string tax_inclusive_amount { get; set; } = default!; 
    public string payable_amount { get; set; } = default!;
}
