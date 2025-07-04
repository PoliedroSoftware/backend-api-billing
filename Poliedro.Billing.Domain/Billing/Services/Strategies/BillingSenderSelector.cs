

using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Domain.Billing.Services.Strategies;

public class BillingSenderSelector(IServiceProvider serviceProvider) : IBillingSenderFactory
{
    public Task<IBillingSenderStrategy> GetSenderAsync(string Provider, string TypeResolution)
    {
        IBillingSenderStrategy sender = (Provider, TypeResolution) switch
        {
            ("PLEMSI", "FE") => serviceProvider.GetRequiredService<IBillingSenderStrategy>(),
            ("PLEMSI", "POS") => serviceProvider.GetRequiredService<IBillingSenderStrategy>(),
            _ => throw new ArgumentException($"Unknown provider ({Provider}) or type ({TypeResolution})")
        };
        return Task.FromResult(sender);

    }
}
