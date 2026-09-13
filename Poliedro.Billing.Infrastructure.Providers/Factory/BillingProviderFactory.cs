using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infrastructure.Providers.Factory;

public class BillingProviderFactory(
    IEnumerable<IBillingProvider> _providers
    ) : IBillingProviderFactory
{
    public IBillingProvider Resolve(
        ProviderType providerType,
        ResolutionType resolutionType)
    {
        var result = _providers.FirstOrDefault(
            x => x.CanHandle(providerType, resolutionType));

        return result
           ?? throw new ArgumentException(
               $"Provider {providerType} no soportado.");
    }
}
