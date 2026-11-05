
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Enums;

namespace Poliedro.Billing.Application.Billing.Ports.Providers;

public interface IBillingProviderFactory
{
    IBillingProvider Resolve(ProviderType providerType, ResolutionType resolutionType);
}
