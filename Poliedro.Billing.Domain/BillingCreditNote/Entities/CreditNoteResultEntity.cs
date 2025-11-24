using Poliedro.Billing.Domain.Client.Enums;
namespace Poliedro.Billing.Domain.BillingCreditNote.Entities;

public class CreditNoteResultEntity
{
    public int ConsecutiveNumber { get; set; }
    public string? Cude { get; set; }
    public string? QRCode { get; set; }
    public string? ConnectionString { get; set; }
    public ProviderType ProviderType { get; set; }
    public int ClientBillingElectronicId { get; set; }
    public string? NumberInvoice { get; set; }
}