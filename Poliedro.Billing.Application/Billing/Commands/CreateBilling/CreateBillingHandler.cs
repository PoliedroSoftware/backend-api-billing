using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Dtos;
using Poliedro.Billing.Application.Billing.Services.Factories.Plemsi;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.CompanyProvider.DomainService;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Commands.CreateBilling;

public class CreateBillingHandler(
    IDianResolutionGetByIdService _dianResolutionGetByIdService,
    ICompanyProviderGetByIdService _companyProviderGetByIdService,
    //IGetProcessorBilling _createBillingFactory,
    //IBillingSenderFactory _billingSenderFactory,
    //IBillingResponseApi _billingResponseApi,
    //IMapper mapper,
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

        //var billingEntities = mapper.Map<IEnumerable<Domain.Billing.CreateBilling>>(request.Invoices);

        //ICreateBilling processor = await _createBillingFactory.GetProcessorAsync(resolutionEntity.ResolutionType, (ProviderType)companyProvider.ProviderId);

        //IEnumerable<(Domain.Billing.CreateBilling Billing, object Output)> processedInvoices = await processor.CreateInvoicesAsync(billingEntities, resolutionEntity, companyProvider, cancellationToken);

        //IBillingSender sender = _billingSenderFactory.Resolve((ProviderType)companyProvider.ProviderId, resolutionEntity.ResolutionType);

        //var billingResults = new List<CreateBillingResultDTO>();

        //foreach (var processed in processedInvoices)
        //{
        //    var invoiceRequest = new PlemsiInvoiceRequest
        //    {
        //        DianResolutionEntity = resolutionEntity,
        //        CompanyProviderEntity = companyProvider,
        //        Invoices = [processed.Output]
        //    };

        //    var responses = await sender.SendAsync(invoiceRequest, cancellationToken);
        //    var result = responses.FirstOrDefault();

        //    if (result is null)
        //    {
        //        _logger.LogWarning("No se recibió respuesta del proveedor para la factura {Number} de la resolución {ResolutionId}.",
        //            processed.Billing.Number, request.Id);
        //        billingResults.Add(new CreateBillingResultDTO
        //        {
        //            Status = false,
        //            Message = "No se recibió respuesta del proveedor para la factura.",
        //            Data = null
        //        });
        //        continue;
        //    }

        //    if (result.Success)
        //    {
        //        try
        //        {
        //            await _billingResponseApi.IBillingResponseApi(
        //                new List<ApiResponseFERetailPos> { result },
        //                new List<Domain.Billing.CreateBilling> { processed.Billing },
        //                resolutionEntity,
        //                companyProvider,
        //                cancellationToken
        //            );

        //            _logger.LogInformation("Factura {Number} emitida y persistida para la resolución {ResolutionId}.",
        //                processed.Billing.Number, request.Id);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error al persistir la factura {Number} de la resolución {ResolutionId}.",
        //                processed.Billing.Number, request.Id);
        //            billingResults.Add(new CreateBillingResultDTO
        //            {
        //                Status = false,
        //                Message = $"La factura fue emitida pero falló la persistencia local: {ex.Message}",
        //                Data = null
        //            });
        //            continue;
        //        }
        //    }
        //    else
        //    {
        //        _logger.LogWarning("Factura {Number} rechazada por el proveedor: {Info}",
        //            processed.Billing.Number, result.Info);
        //    }

        //    billingResults.Add(new CreateBillingResultDTO
        //    {
        //        Status = result.Success,
        //        Message = result.Success ? "Factura procesada exitosamente" : (result.Info ?? "El proveedor rechazó la factura"),
        //        Data = null
        //    });
        //}

        //int successCount = billingResults.Count(r => r.Status);
        //_logger.LogInformation("Emisión completada para la resolución {ResolutionId}: {SuccessCount}/{TotalCount} facturas procesadas.",
        //    request.Id, successCount, billingResults.Count);

        return CreateBillingCommandResult.Ok(billingResults);
    }
}