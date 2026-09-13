using MediatR;
using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    IBillingProviderFactory _billingProviderFactory,
    ILogger<CreateBillingHandler> _logger
    ) : IRequestHandler<CreateBillingCommand, CreateBillingCommandResult>
{
    public async Task<CreateBillingCommandResult> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
    {
        if (request.Invoices is null)
        {
            return CreateBillingCommandResult.BadRequest("El campo 'data' del cuerpo de la petición es obligatorio.");
        }

        var resolution = await _dianResolutionGetByIdService.GetByIdAsync(request.Id, cancellationToken);

        if (resolution is null || resolution.Value is null)
        {
            _logger.LogWarning("Resolución {ResolutionId} no encontrada.", request.Id);
            return CreateBillingCommandResult.NotFound($"No se encontró la resolución {request.Id}.");
        }

        DianResolutionEntity resolutionEntity = resolution.Value;

        var companyProvider = await _companyProviderGetByIdService.GetCompanyProviderByIdAsync(resolutionEntity.CompanyProviderId, cancellationToken);

        if (companyProvider is null)
        {
            _logger.LogWarning("Company provider {CompanyProviderId} de la resolución {ResolutionId} no encontrado.",
                resolutionEntity.CompanyProviderId, request.Id);
            return CreateBillingCommandResult.NotFound($"No se encontró el company provider {resolutionEntity.CompanyProviderId} de la resolución {request.Id}.");
        }

        if (string.IsNullOrWhiteSpace(companyProvider.ApiKey))
        {
            _logger.LogWarning("Company provider {CompanyProviderId} sin ApiKey configurada.",
                companyProvider.CompanyProviderId);
            return CreateBillingCommandResult.BadRequest($"El company provider {companyProvider.CompanyProviderId} no tiene una ApiKey configurada.");
        }

        _logger.LogInformation("Iniciando emisión de {Count} facturas para la resolución {ResolutionId} (provider {ProviderId}).",
            request.Invoices.Count(), request.Id, companyProvider.ProviderId);

        var provider = _billingProviderFactory.Resolve(
            (ProviderType)companyProvider.ProviderId,
            resolutionEntity.ResolutionType);


        var result = await provider.ProcessAsync(
        (IEnumerable<Domain.Billing.CreateBilling>)request.Invoices,
        resolution.Value,
        companyProvider,
        cancellationToken);


        return CreateBillingCommandResult.Ok((IEnumerable<Dtos.CreateBillingResultDTO>)result);
    }
}