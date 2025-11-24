using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;
public interface ICreateCreditNote
{
    Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateCreditNoteAsync(IEnumerable<CreateBilling> invoice, BillingInfoClient ClientInfo, CancellationToken cancellationToken);
}
