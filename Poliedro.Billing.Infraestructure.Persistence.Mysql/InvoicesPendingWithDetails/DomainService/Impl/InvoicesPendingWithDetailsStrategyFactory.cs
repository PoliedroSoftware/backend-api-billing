
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Resolution.Enums;

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
            ResolutionType.Fe => _feRepo,
            ResolutionType.Pos => _posRepo,
            _ => throw new NotSupportedException("Tipo de resolución no soportado")
        };
    }
}
