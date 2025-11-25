using Poliedro.Billing.Application.BillingCreditNote.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.BillingCreditNote.Selectors.Plemsi;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.BillingCreditNote.Impl.Plemsi;

public class BillingCreditNoteSenderFactory : IBillingCreditNoteSenderFactory
{
    private readonly IDictionary<(string, string), IBillingCreditNoteSender> _senders;

    public BillingCreditNoteSenderFactory(IEnumerable<IBillingCreditNoteSender> senders)
    {
        _senders = senders switch
        {
            var list => list.ToDictionary(
                s => s switch
                {
                    BillingCreditNoteSenderFE => ("PLEMSI", "FE"),
                    BillingCreditNoteSenderPOS => ("PLEMSI", "POS"),
                    _ => throw new Exception("Unknown sender type")
                },
                s => s
                )
        };
    }

    public IBillingCreditNoteSender ResolveCreditNote(string provider, ResolutionType typeResolution)
    {
        if (_senders.TryGetValue((provider, typeResolution.ToString()), out var sender))
            return sender;

        throw new InvalidOperationException($"No sender found for {provider}, {typeResolution}");
    }
}
