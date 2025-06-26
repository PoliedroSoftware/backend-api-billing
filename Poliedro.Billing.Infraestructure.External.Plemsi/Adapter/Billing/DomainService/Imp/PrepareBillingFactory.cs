using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.DomainService.Imp;

namespace Polideiro.Billing.Infraestructure.Plemsi.Adapter.Billing.DomainService.Imp;

public class PrepareBillingFactory(IServiceProvider _serviceProvider) : ICreateBillingFactory
{
    public Task<ICreateBillingStrategy> GetProcessorAsync(string resolutyonType, string provider)
    {
        ICreateBillingStrategy strategy = (provider, resolutyonType) switch
        {
            ("1", "FE") => _serviceProvider.GetRequiredService<CreateBillingFERepository>(),
            ("2", "POS") => _serviceProvider.GetRequiredService<CreateBillingPOSRepository>(),
            _ => throw new ArgumentException($"Unknown provider ({provider}) or type ({resolutyonType})")
        };

        return Task.FromResult(strategy);
    }
}
