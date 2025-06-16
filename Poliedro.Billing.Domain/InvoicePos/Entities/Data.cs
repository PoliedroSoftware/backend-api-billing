namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class Data
{
    public int totalDocuments { get; set; }
    public List<Data> docs { get; set; } = default!;
}
