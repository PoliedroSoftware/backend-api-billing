using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Strategies;

public class BillingStrategySelector(
    IServiceProvider serviceProvider
) : ICreateBillingFactory
{
    public Task<ICreateBillingStrategy> GetProcessorAsync(string resolutionType, string provider)
    {
        ICreateBillingStrategy strategy = (provider, resolutionType) switch
        {
            ("PLEMSI", "FE") => serviceProvider.GetRequiredService<PrepareBillingFE>(),
            ("PLEMSI", "POS") => serviceProvider.GetRequiredService<PrepareBillingPOS>(),
            _ => throw new ArgumentException($"Unknown provider ({provider}) or type ({resolutionType})")
        };
        return Task.FromResult(strategy);
    }
}