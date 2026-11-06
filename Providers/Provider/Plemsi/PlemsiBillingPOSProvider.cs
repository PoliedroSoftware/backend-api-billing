using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi
{
    public class PlemsiBillingPOSProvider : IBillingProvider
    {
        public bool CanHandle(ProviderType providerType, ResolutionType resolutionType)
        {
            throw new NotImplementedException();
        }

        public Task<ProviderBillingResult> ProcessAsync(IEnumerable<CreateBilling> invoices, DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
