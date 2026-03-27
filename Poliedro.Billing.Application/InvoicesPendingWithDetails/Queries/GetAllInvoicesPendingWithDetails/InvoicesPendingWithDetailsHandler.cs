using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;

namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

public class InvoicesPendingWithDetailsHandler(
    IClientDomainService clientDomainService,
    IMapper mapper,
    IDatabaseUtils databaseUtils,
    IInvoicesPendingWithDetailsStrategyFactory _strategyFactory
    ) : IRequestHandler<InvoicesPendingWithDetailsQuery, IEnumerable<CreateBillingDTO>>
{
    public async Task<IEnumerable<CreateBillingDTO>> Handle(InvoicesPendingWithDetailsQuery request, CancellationToken cancellationToken)
    {
        var client = await clientDomainService.GetByIdAsync(request.ClientID, request.ProviderType, cancellationToken);

        var repository = _strategyFactory.GetStrategy((Domain.InvoicesPendingWithDetails.Enums.ResolutionType)client.Value.DianResolution.ResolutionType);

        var data = await repository.GetAllInvoicePendingWithDetails(
            client.Value.Server,
            client.Value,
            databaseUtils,
            cancellationToken);

        return mapper.Map<IEnumerable<CreateBillingDTO>>(data);
    }
}
