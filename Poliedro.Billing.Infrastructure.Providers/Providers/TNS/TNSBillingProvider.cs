using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infrastructure.Providers.Providers.TNS;

internal class TNSBillingProvider : IBillingProvider
{
    public bool CanHandle(ProviderType provider, ResolutionType resolutionType)
    {
        return provider == ProviderType.TNS;
    }

    public Task<ProviderBillingResult> ProcessAsync(IEnumerable<CreateBilling> invoices, DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
