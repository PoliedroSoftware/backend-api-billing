using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingResponseApi
{
    Task IBillingResponseApi(List<ApiResponseFERetailPos> response,
        IEnumerable<CreateBilling> processedInvoices,
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken cancellationToken);
}
