using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;
public class BillingSenderFactory : IBillingSenderFactory
{
    private readonly IDictionary<(string, string), IBillingSender> _senders;
    public BillingSenderFactory(IEnumerable<IBillingSender> senders)
    {
        _senders = senders switch
        {
            var list => list.ToDictionary(
                s => s switch
                {
                    BillingSenderFE => ("PLEMSI", "FE"),
                    BillingSenderPOS => ("PLEMSI", "POS"),
                    _ => throw new Exception("Unknown sender type")
                },
                s => s
            )
        };
    }


    public IBillingSender Resolve(ProviderType provider, ResolutionType typeResolution)
    {
        var key = (provider.ToString(), typeResolution.ToString());
        if (_senders.TryGetValue(key, out var sender))
            return sender;

        throw new InvalidOperationException($"No sender found for {provider}, {typeResolution}");
    }
}
