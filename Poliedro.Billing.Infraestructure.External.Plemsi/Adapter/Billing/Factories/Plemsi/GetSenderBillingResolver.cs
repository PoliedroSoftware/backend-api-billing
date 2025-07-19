using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Factories.Plemsi;

class GetSenderBillingResolver(IServiceProvider serviceProvider) : IGetSenderBillingResolver
{

    public Task<object> ResolveSenderAsync(string Provider, string TypeResolution)
    {
        return (Provider, TypeResolution) switch
        {
            ("PLEMSI", "FE") => serviceProvider.GetRequiredService<BillingSenderFE>(), 
            ("PLEMSI", "POS") => serviceProvider.GetRequiredService<BillingSenderPOS>(),

            _ => throw new ArgumentException($"Invalid provider/resolution: {Provider}, {TypeResolution}")
        };
    }
}
