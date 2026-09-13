using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.Billing.Ports.Providers;

public interface IBillingProvider
{
    bool CanHandle(
        ProviderType provider,
        ResolutionType resolutionType);
    Task<ProviderBillingResult> ProcessAsync(
        IEnumerable<CreateBilling> invoices,
        DianResolutionEntity dianResolutionEntity,
        CompanyProviderEntity companyProviderEntity,
        CancellationToken cancellationToken
        );
}
