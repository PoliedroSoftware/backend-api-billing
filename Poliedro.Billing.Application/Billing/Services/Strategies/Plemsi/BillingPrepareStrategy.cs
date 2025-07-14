using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Strategies.Plemsi;

public class BillingPrepareStrategy(
    IServiceProvider _serviceProvider
) : ICreateBillingFactory<CreateBilling, object>
{
    public Task<ICreateBilling<CreateBilling, object>> GetProcessorAsync(string resolutionType, string provider)
    {
        return (provider, resolutionType) switch
        {
            ("PLEMSI", "FE") => Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingFE>()
                        as ICreateBilling<CreateBilling, object>),

            ("PLEMSI", "POS") => Task.FromResult(
                    _serviceProvider.GetRequiredService<PrepareBillingPOS>()
                        as ICreateBilling<CreateBilling, object>),



            _ => throw new ArgumentException($"Unknown provider ({provider}) or type ({resolutionType})")
        };

    }
}