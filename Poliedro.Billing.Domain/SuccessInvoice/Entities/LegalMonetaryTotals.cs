namespace Poliedro.Billing.Domain.SuccessInvoice.Entities;

public class LegalMonetaryTotals
{
    public required string Line_Extension_Amount { get; set; }
    public required string Tax_Exclusive_Amount { get; set; }
    public required string Tax_Inclusive_Amount { get; set; }
    public required string Payable_Amount { get; set; }
}
