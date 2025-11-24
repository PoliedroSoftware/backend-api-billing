using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.BillingCreditNote.Ports;
public interface IResponsesPlemsiCreditNoteRepository
{
    Task IResponsesPlemsiCreditNoteRepositoryAsync(
        List<ApiResponseFERetailPos> response, 
        IEnumerable<CreateBilling> processedInvoices, 
        CancellationToken cancellationToken);
}
