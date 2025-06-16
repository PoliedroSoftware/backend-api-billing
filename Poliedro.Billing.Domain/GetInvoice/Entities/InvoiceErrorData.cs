namespace Poliedro.Billing.Domain.GetInvoice.Entities;
public class InvoiceErrorData
{
    public string IsValid { get; set; }
    public string StatusDescription { get; set; }
    public ErrorMessage ErrorMessage { get; set; }
    public string Filename { get; set; }
}

