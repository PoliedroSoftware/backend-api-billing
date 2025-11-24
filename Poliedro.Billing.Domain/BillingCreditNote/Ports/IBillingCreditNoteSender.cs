using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;
public interface IBillingCreditNoteSender
{
    Task<List<ApiResponseFERetailPos>> SendCreditNoteAsync(PlemsiInvoiceRequest request, BillingInfoClient clientInfo, CancellationToken cancellationToken);
}
