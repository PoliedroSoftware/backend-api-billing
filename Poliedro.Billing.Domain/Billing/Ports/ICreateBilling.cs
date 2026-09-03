using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling
{
    Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(
        IEnumerable<CreateBilling> invoices,
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken cancellationToken);
}