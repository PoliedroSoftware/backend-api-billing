using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
    IInvoicesPendingWithDetailsStrategyFactory _strategyFactory,
    ILogger<InvoicesPendingWithDetailsHandler> _logger
    ) : IRequestHandler<InvoicesPendingWithDetailsQuery, InvoicesPendingWithDetailsResponse?> 
{
    public async Task<InvoicesPendingWithDetailsResponse?> Handle(InvoicesPendingWithDetailsQuery request,CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching pending invoices for resolution {ResolutionId}", request.Id);

        var DianResolutionResult = await _dianResolutionGetByIdService.GetByIdAsync(request.Id, cancellationToken);

        if (DianResolutionResult?.Value is null)
        {
            _logger.LogWarning("Dian resolution {ResolutionId} not found", request.Id);
            return null;
        }

        var CompanyProviderResult = await _companyProviderGetByIdService.GetCompanyProviderByIdAsync(DianResolutionResult.Value.CompanyProviderId, cancellationToken);

        if (CompanyProviderResult is null)
        {
            _logger.LogWarning("Company provider for resolution {ResolutionId} not found", request.Id);
            return null;
        }

        var ServerResult = await _serverGetByIdService.GetByIdAsync(CompanyProviderResult.ServiceId, cancellationToken);

        if (!ServerResult.IsSuccess || ServerResult.Value is null)
        {
            _logger.LogWarning("Server for resolution {ResolutionId} not found", request.Id);
            return null;
        }

        var Repository = _strategyFactory.GetStrategy(DianResolutionResult.Value.ResolutionType);

        var data = await Repository.GetAllInvoicePendingWithDetails(
        ServerResult.Value,
        DianResolutionResult.Value,
        cancellationToken);

        var invoices = mapper.Map<List<CreateBillingDTO>>(data);

        _logger.LogInformation("Found {Count} pending invoices for resolution {ResolutionId}", invoices.Count, request.Id);

        return new InvoicesPendingWithDetailsResponse(true, invoices.Count, invoices);
    }
}