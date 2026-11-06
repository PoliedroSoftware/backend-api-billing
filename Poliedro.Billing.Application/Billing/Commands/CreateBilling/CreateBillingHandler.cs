using MediatR;
using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    IBillingProviderFactory _billingProviderFactory,
    ILogger<CreateBillingHandler> _logger,
    IClientGetByIdService _clientGetByIdService,
    IBillingLicenseAuthorizer _licenseAuthorizer,
    IEmissionFailureRepository _emissionFailures
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

        var client = await _clientGetByIdService.GetByIdAsync(companyProvider.CompanyId, cancellationToken);
        if (!resolutionEntity.Active || companyProvider.Active != 1 || client.Value is not { Active: true })
            return new(false, 403, "El cliente, proveedor o resolución está inactivo.", null);

        var license = await _licenseAuthorizer.AuthorizeAsync(companyProvider.CompanyId, companyProvider.CompanyProviderId, cancellationToken);
        if (!license.Allowed)
            return new(false, 403, "La licencia de facturación no está habilitada: " + license.Code, null);

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

        public Task RecordFailureAsync(int companyId, string? clientName, DianResolutionEntity resolution,
        Domain.Billing.CreateBilling invoice, string? reason, CancellationToken cancellationToken) =>
        _emissionFailures.RecordAsync(new EmissionFailure
        {
            CompanyId = companyId,
            ResolutionId = resolution.ResolutionId,
            ClientName = string.IsNullOrWhiteSpace(clientName) ? "Cliente sin nombre" : clientName[..Math.Min(clientName.Length, 200)],
            InvoiceNumber = invoice.Number[..Math.Min(invoice.Number.Length, 100)],
            Prefix = (invoice.Prefix ?? resolution.Prefix ?? "")[..Math.Min((invoice.Prefix ?? resolution.Prefix ?? "").Length, 30)],
            DocumentType = resolution.ResolutionType.ToString()[..Math.Min(resolution.ResolutionType.ToString().Length, 20)],
            Reason = SafeReason(reason),
            FailedAt = DateTime.UtcNow
        }, cancellationToken);


    private static string SafeReason(string? reason)
    {
        var value = string.IsNullOrWhiteSpace(reason) ? "El proveedor no informó el motivo." : reason.Trim();
        return value.Length <= 2000 ? value : value[..2000];
    }


}


