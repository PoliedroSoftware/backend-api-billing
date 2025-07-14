using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Strategies.Plemsi;

public class BillingPrepareStrategy(
    IServiceProvider serviceProvider
) : ICreateBillingFactory
{
    public Task<ICreateBilling> GetProcessorAsync(string resolutionType, string provider)
    {
        ICreateBilling strategy = (provider, resolutionType) switch
        {
            ("PLEMSI", "FE") => serviceProvider.GetRequiredService<PrepareBillingFE>(),
            ("PLEMSI", "POS") => serviceProvider.GetRequiredService<PrepareBillingPOS>(),
            _ => throw new ArgumentException($"Unknown provider ({provider}) or type ({resolutionType})")
        };
        return Task.FromResult(strategy);
    }
}