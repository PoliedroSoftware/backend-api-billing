namespace Poliedro.Billing.Domain.BillingPos;

public class InvoiceEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = default!;
    public TimeSpan Time { get; set; } = default!;
    public string Resolution { get; set; } = default!;
    public string Prefix { get; set; } = default!;
    public string Number { get; set; } = default!;
    public string ResolutionType { get; set; } = default!;
    public string Note { get; set; } = default!;
    public decimal AllowanceTotal { get; set; } = default!;
    public decimal InvoiceBaseTotal { get; set; } = default!;
    public decimal InvoiceTaxExclusiveTotal { get; set; } = default!;
    public decimal InvoiceTaxInclusiveTotal { get; set; } = default!;
    public decimal TotalToPay { get; set; } = default!;
    public decimal FinalTotalToPay { get; set; } = default!;
}
