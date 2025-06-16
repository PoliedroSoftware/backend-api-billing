using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.InvoicesPendingWithDetails.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

public class InvoicesPendingWithDetailsHandler(
    IClientDomainService clientDomainService,
    IMapper mapper,
    IDatabaseUtils databaseUtils,
    IInvoicesPendingWithDetailsStrategyFactory _strategyFactory
    ) : IRequestHandler
    <InvoicesPendingWithDetailsQuery,
    IEnumerable<object>>
{
    public async Task<IEnumerable<object>> Handle(InvoicesPendingWithDetailsQuery request,CancellationToken cancellationToken)
    {
        var client = await clientDomainService.GetByIdAsync(request.ApiKey, cancellationToken);

        if (client == null) throw new KeyNotFoundException("Cliente no encontrado.");

        var repository = _strategyFactory.GetStrategy((Domain.InvoicesPendingWithDetails.Enums.ResolutionType)client.Value.DianResolution.ResolutionType);


        var data = await repository.GetAllInvoicePendingWithDetails(
        client.Value.Server,
        client.Value,
        databaseUtils,
        cancellationToken,
        request.ApiKey);

        return mapper.Map<IEnumerable<object>>(data);


    }
}