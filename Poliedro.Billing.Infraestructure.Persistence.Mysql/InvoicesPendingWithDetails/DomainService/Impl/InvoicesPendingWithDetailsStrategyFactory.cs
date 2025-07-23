using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Enums;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;

public class InvoicesPendingWithDetailsStrategyFactory
    (
    InvoicesPendingWithDetailsFERepository _feRepo,
    InvoicesPendingWithDetailsPOSRepository _posRepo
    ) : IInvoicesPendingWithDetailsStrategyFactory
{

    public IInvoicesPendingWithDetailsStrategy GetStrategy(ResolutionType resolutionType)
    {
        return resolutionType switch
        {
            ResolutionType.FE => _feRepo,
            ResolutionType.POS => _posRepo,
            _ => throw new NotSupportedException("Tipo de resolución no soportado")
        };
    }
}
