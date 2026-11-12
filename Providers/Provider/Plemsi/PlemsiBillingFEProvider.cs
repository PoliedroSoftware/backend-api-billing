using AutoMapper;
using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Application.Billing.Ports.Providers.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi
{
    /// <summary>
    /// Proveedor de facturación para Plemsi en formato FE (Factura Electrónica).
    /// Orquesta la transformación de facturas, la asignación del consecutivo del
    /// proveedor y el envío, normalizando las respuestas.
    /// </summary>
    public class PlemsiBillingFEProvider : IBillingProvider
    {
        private readonly ICreateBilling _preparador;
        private readonly ISenderPlemsi _sender;
        private readonly IConsecutiveProvider _consecutive;
        private readonly IMapper _mapper;
        private readonly ILogger<PlemsiBillingFEProvider> _logger;

        public PlemsiBillingFEProvider(
            ICreateBilling preparador,
            ISenderPlemsi sender,
            IConsecutiveProvider consecutive,
            IMapper mapper,
            ILogger<PlemsiBillingFEProvider> logger)
        {
            _preparador = preparador;
            _sender = sender;
            _consecutive = consecutive;
            _mapper = mapper;
            _logger = logger;
        }

        public bool CanHandle(ProviderType provider, ResolutionType resolutionType)
        {
            return provider == ProviderType.PLEMSI && resolutionType == ResolutionType.FE;
        }

        public async Task<ProviderBillingResult> ProcessAsync(
            IEnumerable<CreateBilling> invoices,
            DianResolutionEntity dianResolutionEntity,
            CompanyProviderEntity companyProviderEntity,
            CancellationToken cancellationToken)
        {
            var billingResultItems = new List<BillingResultItem>();

            try
            {
                _logger.LogInformation("Iniciando procesamiento de {Count} facturas FE en Plemsi",
                    invoices.Count());

                // ===== PASO 1: TRANSFORMAR (sin consecutivo) =====
                var prepared = await _preparador.CreateInvoicesAsync(
                    invoices,
                    dianResolutionEntity,
                    companyProviderEntity,
                    cancellationToken);

                var preparedList = prepared.ToList();
                _logger.LogInformation("Facturas preparadas: {Count}", preparedList.Count);

                if (preparedList.Count == 0)
                {
                    return new ProviderBillingResult
                    {
                        Success = false,
                        Message = "Todas las facturas fallaron en la preparación",
                        Results = billingResultItems
                    };
                }

                // Separar preparadas exitosamente de las que fallaron
                var eligible = new List<(CreateBilling Billing, FERetailelectronicEntity Document)>();

                foreach (var preparedInvoice in preparedList)
                {
                    if (preparedInvoice.Error is not null)
                    {
                        _logger.LogWarning("Factura {Number} falló en preparación: {Error}",
                            preparedInvoice.Billing.Number, preparedInvoice.Error);

                        billingResultItems.Add(Failure(preparedInvoice.Billing.Number, preparedInvoice.Error));
                        continue;
                    }

                    if (preparedInvoice.Output is not FERetailelectronicEntity document)
                    {
                        billingResultItems.Add(Failure(
                            preparedInvoice.Billing.Number,
                            "No se pudo construir el documento para Plemsi."));
                        continue;
                    }

                    eligible.Add((preparedInvoice.Billing, document));
                }

                if (eligible.Count == 0)
                {
                    return new ProviderBillingResult
                    {
                        Success = false,
                        Message = "Todas las facturas fallaron en la preparación",
                        Results = billingResultItems
                    };
                }

                // ===== PASO 2: ASIGNAR CONSECUTIVO Y ENVIAR =====
                // El consecutivo es una condición del proveedor: se pide justo antes
                // de cada envío, no durante la preparación.
                for (var i = 0; i < eligible.Count; i++)
                {
                    var (billing, document) = eligible[i];

                    var consecutive = await _consecutive.NextAsync(
                        dianResolutionEntity,
                        companyProviderEntity,
                        cancellationToken);

                    if (!consecutive.Success)
                    {
                        _logger.LogWarning("No se pudo obtener consecutivo para la factura {Number}: {Reason}",
                            billing.Number, consecutive.ErrorCode);

                        // Falla de resolución (vencida/agotada/sin proveedor): el resto del lote tampoco puede emitirse.
                        for (var j = i; j < eligible.Count; j++)
                        {
                            billingResultItems.Add(Failure(eligible[j].Billing.Number, consecutive.Message));
                        }

                        break;
                    }

                    document.number = consecutive.Number;

                    var dto = _mapper.Map<SenderRequestFEDTO>(document);

                    billingResultItems.Add(await _sender.SendAsync(
                        dto,
                        dianResolutionEntity,
                        companyProviderEntity,
                        cancellationToken));
                }

                // ===== PASO 3: NORMALIZAR y RETORNAR =====
                return NormalizeResults(billingResultItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico procesando facturas FE en Plemsi");
                return new ProviderBillingResult
                {
                    Success = false,
                    Message = $"Error procesando facturas: {ex.Message}",
                    Results = billingResultItems
                };
            }
        }

        private static BillingResultItem Failure(string? invoiceNumber, string? message) => new()
        {
            InvoiceNumber = invoiceNumber,
            Success = false,
            Message = string.IsNullOrWhiteSpace(message) ? "Error desconocido" : message
        };

        /// <summary>
        /// Normaliza los resultados para retornar al endpoint.
        /// </summary>
        private ProviderBillingResult NormalizeResults(List<BillingResultItem> items)
        {
            var successCount = items.Count(x => x.Success);
            var failureCount = items.Count(x => !x.Success);

            var result = new ProviderBillingResult
            {
                Success = failureCount == 0,
                Message = $"Procesadas {successCount} exitosas, {failureCount} fallidas",
                Results = items
            };

            _logger.LogInformation("Procesamiento completado. Éxito: {Success}, Exitosas: {Successful}, Fallidas: {Failed}",
                result.Success, successCount, failureCount);

            return result;
        }
    }
}
