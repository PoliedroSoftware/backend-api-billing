using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.BillingPos.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.BillingPos.Ports;

namespace Poliedro.Billing.Application.BillingPos.Services.Strategies;

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