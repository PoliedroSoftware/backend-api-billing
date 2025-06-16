namespace Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;

public class InvoiceElectronic
{
    public int Id { get; set; }
    public string? Identication { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Invoice { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? TransactionDate { get; set; }
    public int AllowanceTotal { get; set; }
    public decimal InvoiceBaseTotal { get; set; }
    public decimal InvoiceTaxExclusiveTotal { get; set; }
    public decimal InvoiceTaxInclusiveTotal { get; set; }
    public decimal TotalToPay { get; set; }
    public string? CreatedByName { get; set; }
    public List<InvoiceWithDetailElectronic> InvoiceElectronicDetails { get; set; } = new();

}