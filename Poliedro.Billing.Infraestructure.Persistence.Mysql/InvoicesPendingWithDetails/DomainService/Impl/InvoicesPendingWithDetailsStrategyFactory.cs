
using Poliedro.Billing.Domain.InvoicePendingWithDetails.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Enums;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;

public class InvoicesPendingWithDetailsStrategyFactory : IInvoicesPendingWithDetailsStrategyFactory
{
    private readonly InvoicesPendingWithDetailsFERepository _feRepo;
    private readonly InvoicesPendingWithDetailsPOSRepository _posRepo;

    public InvoicesPendingWithDetailsStrategyFactory(
        InvoicesPendingWithDetailsFERepository feRepo,
        InvoicesPendingWithDetailsPOSRepository posRepo)
    {
        _feRepo = feRepo;
        _posRepo = posRepo;
    }

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
