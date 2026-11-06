using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Enums;



namespace Poliedro.Billing.Infraestructure.External.Provider.Factory
{
    public class BillingProviderFactory : IBillingProviderFactory
    {
        public IBillingProvider Resolve(ProviderType providerType, ResolutionType resolutionType)
        {
            throw new NotImplementedException();
        }
    }
}
