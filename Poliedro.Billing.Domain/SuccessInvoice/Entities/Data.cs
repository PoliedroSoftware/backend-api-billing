namespace Poliedro.Billing.Domain.SuccessInvoice.Entities;

public class Data
{
    public int TotalDocuments { get; set; }
    public List<SuccessInvoice> Docs { get; set; } = [];
    public int Page { get; set; }
    public int PerPage { get; set; }
}
