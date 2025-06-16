namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;

public class InvoiceFEPendingEntity
{
    public int Id { get; set; }
    public string Identication { get; set; }
    public string Contact_name { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Invoice { get; set; }
    public string Payment_status { get; set; }
    public string Transaction_date { get; set; }
    public double AllowanceTotal { get; set; }
    public double InvoiceBaseTotal { get; set; }
    public double InvoiceTaxExclusiveTotal { get; set; }
    public double InvoiceTaxInclusiveTotal { get; set; }
    public double TotalToPay { get; set; }
    public string Created_by_name { get; set; }
    public double Percent { get; set; }
    public double InvoiceTax { get; set; }
    public int SendDian { get; set; }

    public List<DetailsInvoiceFEPendingEntity> DetailsInvoicePendings { get; set; } = new();
}
