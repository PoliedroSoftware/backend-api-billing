
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

public interface IInvoicesPendingWithDetailsStrategyFactory
{
    IInvoicesPendingWithDetailsStrategy GetStrategy(ResolutionType resolutionType);
}
