using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling
{
    Task<IEnumerable<BillingPreparationResult>> CreateInvoicesAsync(
        IEnumerable<CreateBilling> invoices,
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        int lastInvoiceNumber,
        CancellationToken cancellationToken);
}
