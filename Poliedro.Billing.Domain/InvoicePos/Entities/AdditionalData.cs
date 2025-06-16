namespace Poliedro.Billing.Domain.InvoicePos.Entities;

public class AdditionalData
{
    public SoftwareManufacturer software_manufacturer { get; set; } = default!;
    public PayPoint pay_point { get; set; } = default!;
}
