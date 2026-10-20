using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;

public class BillingPrepareFactory(IServiceProvider _serviceProvider) : IGetProcessorBilling
{
    public Task<ICreateBilling> GetProcessorAsync(ResolutionType resolutionType, ProviderType provider)
    {
        return (provider, resolutionType) switch
        {
            (ProviderType.PLEMSI, ResolutionType.FE) =>
                Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingFE>()
                    as ICreateBilling),

            (ProviderType.PLEMSI, ResolutionType.POS) =>
                Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingPOS>()
                    as ICreateBilling),

            _ => throw new ArgumentException(
                $"Unknown provider ({provider}) or type ({resolutionType})")
        };

    }
}