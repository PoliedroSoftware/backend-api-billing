using AutoMapper;
using MediatR;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Server.DomainService;

namespace Poliedro.Billing.Application.InvoicesPendingWithDetails.Queries.GetAllInvoicesPendingWithDetails;

public class InvoicesPendingWithDetailsHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    IServerGetByIdService _serverGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    IMapper mapper,
    IDatabaseUtils databaseUtils,
    IInvoicesPendingWithDetailsStrategyFactory _strategyFactory
    ) : IRequestHandler<InvoicesPendingWithDetailsQuery, IEnumerable<CreateBillingDTO>> 
{
    public async Task<IEnumerable<CreateBillingDTO>> Handle(InvoicesPendingWithDetailsQuery request,CancellationToken cancellationToken)
    {
        var DianResolutionResult = await _dianResolutionGetByIdService.GetByIdAsync(request.Id, cancellationToken);

        var CompanyProviderResult = await _companyProviderGetByIdService.GetCompanyProviderByIdAsync(DianResolutionResult.Value.CompanyProviderId, cancellationToken);

        var ServerResult = await _serverGetByIdService.GetByIdAsync(CompanyProviderResult.CompanyId, cancellationToken);

        var Repository = _strategyFactory.GetStrategy(DianResolutionResult.Value.ResolutionType);

        var data = await Repository.GetAllInvoicePendingWithDetails(
        ServerResult.Value,
        DianResolutionResult.Value,
        cancellationToken);

        return mapper.Map<IEnumerable<CreateBillingDTO>>(data);
    }
}