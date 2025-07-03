using Poliedro.Billing.Application.BillingPos.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Domain.Billing.Services.Strategies;

public class BillingStrategySender : IBillingSenderFactory
{
    private readonly BillingFESender _billingFESender;

    public BillingStrategySender(BillingFESender billingFESender)
    {
        _billingFESender = billingFESender;
    }

    public IBillingSender GetSender(string typeResolution, string providerType)
    {
        // Aquí decides cuál sender devolver según providerType (y opcionalmente typeResolution)
        return providerType switch
        {
            "PLEMSI" => _billingFESender,
            // Agregas más: "TNS" => _billingTnsSender,
            _ => throw new NotSupportedException($"Proveedor '{providerType}' no soportado")
        };
    }
}