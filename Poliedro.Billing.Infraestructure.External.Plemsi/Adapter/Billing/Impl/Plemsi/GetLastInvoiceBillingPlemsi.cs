using System.Threading;
using System.Threading.Tasks;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

public class GetLastInvoiceBillingPlemsi : IGetLastInvoiceBilling
{
    public Task<int> GetLastInvoiceNumberAsync(DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken)
    {
        // simple default implementation for startup; real logic should query provider
        return Task.FromResult(1);
    }
}
