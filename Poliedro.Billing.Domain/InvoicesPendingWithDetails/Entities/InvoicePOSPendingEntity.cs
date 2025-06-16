
namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;

public class InvoicePOSPendingEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Resolution { get; set; }
    public string Prefix { get; set; }
    public string Number { get; set; }
    public string ResolutionType { get; set; }
    public string Note { get; set; }
    public decimal AllowanceTotal { get; set; }
    public decimal InvoiceBaseTotal { get; set; }
    public decimal InvoiceTaxExclusiveTotal { get; set; }
    public decimal InvoiceTaxInclusiveTotal { get; set; }
    public decimal TotalToPay { get; set; }

    public List<DetailsInvoicePOSPendingEntity> DetailsInvoicePendings { get; set; } = new();

}
