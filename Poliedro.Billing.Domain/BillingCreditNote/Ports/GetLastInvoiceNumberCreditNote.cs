
using Poliedro.Billing.Domain.Billing;

namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;

public interface GetLastInvoiceNumberCreditNote
{
    Task<int> GetLastInvoiceNumberCreditNoteAsync(BillingInfoClient clientInfo, CancellationToken cancellationToken);
}
