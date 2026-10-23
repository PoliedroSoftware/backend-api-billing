using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Application.Billing.Services.Ports;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Client.DomainService;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    IGetProcessorBilling _createBillingFactory,
    IBillingProviderAdapterFactory _billingProviderAdapterFactory,
    IBillingEmissionResultRecorder _billingEmissionResultRecorder,
    IMapper mapper,
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

        _logger.LogInformation("Iniciando emisión de {Count} facturas para la resolución {ResolutionId} (provider {ProviderId}).",
            request.Invoices.Count(), request.Id, companyProvider.ProviderId);

        var billingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        var providerId = companyProvider.ProviderId;
        IBillingProviderAdapter providerAdapter;
        try
        {
            providerAdapter = _billingProviderAdapterFactory.Resolve(providerId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "No hay adaptador de facturación configurado para el proveedor {ProviderId}.", companyProvider.ProviderId);
            return CreateBillingCommandResult.BadRequest("El proveedor de facturación configurado no está disponible.");
        }

        var configurationValidation = providerAdapter.ValidateConfiguration(companyProvider);
        if (!configurationValidation.IsValid)
        {
            _logger.LogWarning("Configuración inválida para proveedor {ProviderId}, company provider {CompanyProviderId}: {Reason}",
                companyProvider.ProviderId, companyProvider.CompanyProviderId, configurationValidation.Error);
            return CreateBillingCommandResult.BadRequest(configurationValidation.Error ?? "La configuración del proveedor no es válida.");
        }

        ICreateBilling processor = await _createBillingFactory.GetProcessorAsync(resolutionEntity.ResolutionType);
        IReadOnlyList<Domain.Billing.BillingPreparationResult> processedInvoices;
        try
        {
            var lastInvoiceNumber = await providerAdapter.GetLastNumberAsync(resolutionEntity, companyProvider, cancellationToken);
            processedInvoices = (await processor.CreateInvoicesAsync(
                billingEntities, resolutionEntity, companyProvider, lastInvoiceNumber, cancellationToken)).ToArray();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "No hay numerador configurado para proveedor {ProviderId} y documento {DocumentType}.", providerId, resolutionEntity.ResolutionType);
            return CreateBillingCommandResult.BadRequest("No está configurada la numeración para el proveedor de facturación.");
        }

        var billingResults = new List<CreateBillingResultDTO>();

        foreach (var processed in processedInvoices)
        {
            if (processed.Error is not null)
            {
                _logger.LogWarning("Factura {Number} no emitida: {Error}",
                    processed.Billing.Number, processed.Error);
                billingResults.Add(new CreateBillingResultDTO
                {
                    Status = false,
                    Message = processed.Error,
                    Data = null
                });
                continue;
            }

            license = await _licenseAuthorizer.AuthorizeAsync(companyProvider.CompanyId, companyProvider.CompanyProviderId, cancellationToken);
            if (!license.Allowed)
            {
                billingResults.Add(new CreateBillingResultDTO { Status = false, Message = "Licencia suspendida o no disponible.", Data = null });
                break;
            }
            IReadOnlyList<Domain.Billing.BillingProviderResponse> responses;
            try
            {
                responses = await providerAdapter.SendAsync(new BillingProviderSubmission(
                    resolutionEntity,
                    companyProvider,
                    resolutionEntity.ResolutionType,
                    [processed.Output!]), cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                var reason = SafeReason(ex.Message);
                await RecordFailureAsync(companyProvider.CompanyId, client.Value?.Name, resolutionEntity,
                    processed.Billing, reason, cancellationToken);
                _logger.LogError(ex, "Error enviando factura {Number} a DIAN para la resolución {ResolutionId}.",
                    processed.Billing.Number, request.Id);
                billingResults.Add(new CreateBillingResultDTO { Status = false, Message = reason, Data = null });
                continue;
            }
            var result = responses.FirstOrDefault();

            if (result is null)
            {
                _logger.LogWarning("No se recibió respuesta del proveedor para la factura {Number} de la resolución {ResolutionId}.",
                    processed.Billing.Number, request.Id);
                await RecordFailureAsync(companyProvider.CompanyId, client.Value?.Name, resolutionEntity,
                    processed.Billing, "No se recibió respuesta del proveedor; verificar el estado con el proveedor antes de reintentar.", cancellationToken);
                billingResults.Add(new CreateBillingResultDTO
                {
                    Status = false,
                    Message = "No se recibió respuesta del proveedor para la factura.",
                    Data = null
                });
                continue;
            }

            if (result.Success)
            {
                try
                {
                    await _billingEmissionResultRecorder.RecordAsync(
                        new List<Domain.Billing.BillingProviderResponse> { result },
                        new List<Domain.Billing.CreateBilling> { processed.Billing },
                        resolutionEntity,
                        companyProvider,
                        cancellationToken
                    );

                    _logger.LogInformation("Factura {Number} emitida y persistida para la resolución {ResolutionId}.",
                        processed.Billing.Number, request.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al persistir la factura {Number} de la resolución {ResolutionId}.",
                        processed.Billing.Number, request.Id);
                    billingResults.Add(new CreateBillingResultDTO
                    {
                        Status = false,
                        Message = $"La factura fue emitida pero falló la persistencia local: {ex.Message}",
                        Data = null
                    });
                    continue;
                }
            }
            else
            {
                _logger.LogWarning("Factura {Number} rechazada por el proveedor: {Info}",
                    processed.Billing.Number, result.Message);
                await RecordFailureAsync(companyProvider.CompanyId, client.Value?.Name, resolutionEntity,
                    processed.Billing, SafeReason(result.Message), cancellationToken);
            }

            billingResults.Add(new CreateBillingResultDTO
            {
                Status = result.Success,
                Message = result.Success ? "Factura procesada exitosamente" : (result.Message ?? "El proveedor rechazó la factura"),
                Data = null
            });
        }

        int successCount = billingResults.Count(r => r.Status);
        _logger.LogInformation("Emisión completada para la resolución {ResolutionId}: {SuccessCount}/{TotalCount} facturas procesadas.",
            request.Id, successCount, billingResults.Count);

        return CreateBillingCommandResult.Ok(billingResults);
    }

    private Task RecordFailureAsync(int companyId, string? clientName, DianResolutionEntity resolution,
        Domain.Billing.CreateBilling invoice, string? reason, CancellationToken cancellationToken) =>
        _emissionFailures.RecordAsync(new EmissionFailure
        {
            CompanyId = companyId,
            ResolutionId = resolution.ResolutionId,
            ClientName = string.IsNullOrWhiteSpace(clientName) ? "Cliente sin nombre" : clientName[..Math.Min(clientName.Length, 200)],
            InvoiceNumber = invoice.Number[..Math.Min(invoice.Number.Length, 100)],
            Prefix = (invoice.Prefix ?? resolution.Prefix ?? "")[..Math.Min((invoice.Prefix ?? resolution.Prefix ?? "").Length, 30)],
            DocumentType = resolution.ResolutionType.ToString()[..Math.Min(resolution.ResolutionType.ToString().Length, 20)],
            CommercialMessage = Commercialize(reason),
            Reason = SafeReason(reason),
            FailedAt = DateTime.UtcNow
        }, cancellationToken);

    private static string Commercialize(string? reason)
    {
        var value = (reason ?? "").Trim().ToLowerInvariant();
        if (value.Contains("no se recibió respuesta") || value.Contains("no se recibio respuesta")
            || value.Contains("timeout") || value.Contains("timed out") || value.Contains("time-out"))
            return "No recibimos confirmación de la factura. Verifique su estado en Plemsi antes de intentar enviarla de nuevo.";
        if (value.Contains("unauthorized") || value.Contains("forbidden") || value.Contains("api key")
            || value.Contains("apikey") || value.Contains("token") || value.Contains("401"))
            return "No fue posible validar el acceso al proveedor de facturación. El equipo de soporte debe revisar la conexión.";
        if (value.Contains("reject") || value.Contains("rechaz") || value.Contains("dian"))
            return "La factura no fue aceptada por el proveedor o la DIAN. Revise el detalle técnico antes de corregirla y reenviarla.";
        if (value.Contains("invalid") || value.Contains("validation") || value.Contains("validaci")
            || value.Contains("obligatorio") || value.Contains("requerido") || value.Contains("no válido")
            || value.Contains("no valido"))
            return "La factura contiene información que requiere revisión. Consulte el detalle técnico para identificar el dato que debe corregirse.";
        if (value.Contains("connection") || value.Contains("conexi") || value.Contains("socket")
            || value.Contains("network") || value.Contains("red"))
            return "No fue posible conectar con el proveedor de facturación. Verifique el estado del servicio antes de reenviar.";
        return "No se pudo confirmar la emisión de esta factura. El equipo de soporte debe revisar el detalle técnico antes de reenviarla.";
    }

    private static string SafeReason(string? reason)
    {
        var value = string.IsNullOrWhiteSpace(reason) ? "El proveedor no informó el motivo." : reason.Trim();
        return value.Length <= 2000 ? value : value[..2000];
    }
}
