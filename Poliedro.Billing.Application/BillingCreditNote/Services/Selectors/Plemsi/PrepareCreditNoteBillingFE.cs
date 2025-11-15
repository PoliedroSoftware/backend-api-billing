using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;

namespace Poliedro.Billing.Application.BillingCreditNote.Services.Selectors.Plemsi
{
    public class PrepareCreditNoteBillingFE : ICreateCreditNote
    {
        public Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateCreditNoteAsync(IEnumerable<CreateBilling> invoice, BillingInfoClient ClientInfo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
