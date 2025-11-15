using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;

namespace Poliedro.Billing.Application.BillingCreditNote.Services.Selectors.Plemsi;

public class PrepareCreditNoteBillingPOS : ICreateCreditNote // Requiere Menos información
{
    public Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateCreditNoteAsync(IEnumerable<CreateBilling> invoice, BillingInfoClient ClientInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
