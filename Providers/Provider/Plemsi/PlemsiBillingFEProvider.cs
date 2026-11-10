using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi
{
    /// <summary>
    /// Proveedor de facturación para Plemsi en formato FE (Factura Electrónica).
    /// Orquesta la transformación de facturas, envío a Plemsi y normalización de respuestas.
    /// </summary>
    public class PlemsiBillingFEProvider : IBillingProvider
    {
        private readonly PrepareBillingFE _preparador;
        private readonly ISenderPlemsi _sender;
        private readonly ILogger<PlemsiBillingFEProvider> _logger;

        public PlemsiBillingFEProvider(
            PrepareBillingFE preparador,
            ISenderPlemsi sender,
            ILogger<PlemsiBillingFEProvider> logger)
        {
            _preparador = preparador;
            _sender = sender;
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
            try
            {
                _logger.LogInformation("Iniciando procesamiento de {Count} facturas FE en Plemsi", 
                    invoices.Count());

                // ===== PASO 1: TRANSFORMAR =====
                // Convierte CreateBilling → DTO de Plemsi usando PrepareBillingFE
                var billingResults = await _preparador.CreateInvoicesAsync(
                    invoices,
                    dianResolutionEntity,
                    companyProviderEntity,
                    cancellationToken);

                var resultsList = billingResults.ToList();
                _logger.LogInformation("Facturas preparadas: {Count}", resultsList.Count);

                // Separar preparadas exitosamente de las que fallaron
                var successResults = resultsList.Where(x => x.Error == null).ToList();
                var failedResults = resultsList.Where(x => x.Error != null).ToList();

                var billingResultItems = new List<BillingResultItem>();

                // Registrar fallos en la preparación
                foreach (var failed in failedResults)
                {
                    _logger.LogWarning("Factura {Number} falló en preparación: {Error}",
                        failed.Billing.Number, failed.Error);

                    billingResultItems.Add(new BillingResultItem
                    {
                        InvoiceNumber = failed.Billing.Number,
                        Success = false,
                        Message = failed.Error
                    });
                }

                if (!successResults.Any())
                {
                    return new ProviderBillingResult
                    {
                        Success = false,
                        Message = "Todas las facturas fallaron en la preparación",
                        Results = billingResultItems
                    };
                }

                // ===== PASO 3: ENVIAR a Plemsi =====
                _logger.LogInformation("Preparadas {Count} facturas. Iniciando envío a Plemsi FE", 
                    successResults.Count);

                // El sender maneja: envío HTTP + normalización de respuesta
                var sentResults = await _sender.SendAndNormalizeAsync(
                    successResults.Select(x => x.Output!),
                    dianResolutionEntity,
                    companyProviderEntity,
                    cancellationToken);

                billingResultItems.AddRange(sentResults);

                // ===== PASO 5: NORMALIZAR y RETORNAR =====
                return NormalizeResults(billingResultItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico procesando facturas FE en Plemsi");
                return new ProviderBillingResult
                {
                    Success = false,
                    Message = $"Error procesando facturas: {ex.Message}",
                    Results = []
                };
            }
        }

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
