using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IInvoiceLastPos
{
    Task<int> GetInvoiceLastAsync(DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken);
}
