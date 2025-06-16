namespace Poliedro.Billing.Domain.BillingPos;

public class InvoiceSuccessEntity
{
    public int Id { get; set; }
    public string Invoice { get; set; }
    public int ProviderId { get; set; }
    public DateTime? DateRegister { get; set; }
    public string Cude { get; set; }
    public string QrCode { get; set; }
    public int ClientId { get; set; }
}
