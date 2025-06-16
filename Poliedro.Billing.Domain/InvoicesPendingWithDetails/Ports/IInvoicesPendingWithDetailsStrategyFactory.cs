using Poliedro.Billing.Domain.InvoicePendingWithDetails.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Enums;

namespace Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

public interface IInvoicesPendingWithDetailsStrategyFactory
{
    IInvoicesPendingWithDetailsStrategy GetStrategy(ResolutionType resolutionType);
}
