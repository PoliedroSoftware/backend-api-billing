using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.BillingCreditNote.Selectors.Plemsi;

public class BillingCreditNoteSenderPOS : IBillingCreditNoteSender
{
    public Task<List<ApiResponseFERetailPos>> SendCreditNoteAsync(PlemsiInvoiceRequest request, BillingInfoClient clientInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
