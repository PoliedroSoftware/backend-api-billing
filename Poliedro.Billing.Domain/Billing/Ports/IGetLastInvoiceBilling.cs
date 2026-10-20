using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IGetLastInvoiceBilling
{
    Task<int> GetLastInvoiceNumberAsync(DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken);
}
