using MediatR;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;

namespace Poliedro.Billing.Api.Observability.Pipeline;

/// <summary>
/// Pipeline Behavior de MediatR que instrumenta handlers de facturación.
/// 
/// Ubicación: Observability layer, registrado como Pipeline Behavior en MediatR.
/// Propósito: Interceptar requests/responses de MediatR para registrar:
/// - billing_invoice_success_total
/// - billing_invoice_failed_total
/// - billing_invoice_duration_seconds
/// 
/// ¿Por qué Pipeline Behavior?
/// ✓ Automáticamente instrumenta TODOS los CreateBillingCommand handlers sin tocar endpoints
/// ✓ Captura tanto requests exitosos como excepciones (catch en el try/finally)
/// ✓ Centraliza lógica de observabilidad fuera de la lógica de negocio
/// ✓ Fácilmente extensible para otros handlers (agregar más IRequest checks)
/// ✓ Respeta arquitectura Clean: Application layer contiene handlers, esta clase es middleware
/// 
/// Flujo:
/// 1. Request entra (CreateBillingCommand)
/// 2. Registramos startTime y extraemos labels
/// 3. Ejecutamos handler original (await next())
/// 4. Si éxito → InvoiceSuccessTotal++ + log
/// 5. Si excepción → catch, categorizar error, InvoiceFailedTotal++, re-throw
/// 6. Siempre (finally) → registrar duración en Histogram
/// </summary>
public class MetricsObservabilityBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<MetricsObservabilityBehavior<TRequest, TResponse>> _logger;

    public MetricsObservabilityBehavior(ILogger<MetricsObservabilityBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Solo instrumentar CreateBillingCommand
        // Otros handlers pasan directo (no agregar overhead innecesario)
        if (request is not CreateBillingCommand billingRequest)
        {
            return await next();
        }

        var startTime = DateTime.UtcNow;
        var labels = ExtractLabels(billingRequest);

        try
        {
            _logger.LogInformation(
                "Iniciando emisión de factura - Provider: {Provider}, ClientId: {ClientId}, InvoiceType: {InvoiceType}",
                labels.Provider, labels.ClientId, labels.InvoiceType);

            // Ejecutar handler
            var response = await next();

            // No registrar contadores de éxito/fallo aquí para evitar double-counting
            // Los Decorators de proveedores registran éxito/fallo con labels exactos.

            _logger.LogInformation(
                "Factura procesada por handler - ClientId: {ClientId}",
                labels.ClientId);

            return response;
        }
        catch (Exception ex)
        {
            // Categorizar error y registrar fallos
            var errorReason = CategorizeError(ex);

            _logger.LogError(
                ex,
                "Fallo en emisión de factura - ClientId: {ClientId}, Error: {ErrorReason}",
                labels.ClientId, errorReason);

            // Re-lanzar para que el error se propague (ya está logeado)
            throw;
        }
        finally
        {
            // Registrar duración SIEMPRE (éxito o error)
            // El Histogram captures ambos casos
            var duration = DateTime.UtcNow.Subtract(startTime).TotalSeconds;
            BillingMetrics.InvoiceDurationSeconds
                .WithLabels(labels.Provider, labels.ClientId, labels.InvoiceType)
                .Observe(duration);

            _logger.LogInformation(
                "Emisión de factura completada - Duración: {DurationSeconds}s",
                duration);
        }
    }

    /// <summary>
    /// Extrae provider, client_id e invoice_type del request.
    /// 
    /// Nota: provider se determina en el handler después de consultar BD.
    /// Aquí usamos heurística: será sobrescrito cuando CallableContexts lo sepa.
    /// client_id viene del request.ApiKey (es un identificador único del cliente).
    /// invoice_type se determina según tipo de resolución DIAN.
    /// </summary>
    private static (string Provider, string ClientId, string InvoiceType) ExtractLabels(CreateBillingCommand request)
    {
        var provider = "pending"; // Se actualiza en el handler
        var clientId = request.ApiKey?.Substring(0, Math.Min(8, request.ApiKey.Length)) ?? "unknown";
        var invoiceType = "FE"; // FE = Factura Electrónica (default)

        return (provider, clientId, invoiceType);
    }

    /// <summary>
    /// Categoriza excepciones en buckets para análisis.
    /// 
    /// Categorías:
    /// - validation_error: datos inválidos, violación de reglas de negocio
    /// - provider_error: timeout, 5xx, malformed response, auth failed
    /// - business_error: lógica de dominio (cliente no existe, límites, etc)
    /// - timeout_error: específicamente timeouts
    /// - unknown_error: sin categoría
    /// </summary>
    private static string CategorizeError(Exception ex)
    {
        if (ex is null)
            return "unknown_error";

        var exceptionType = ex.GetType().Name;

        // Validación de FluentValidation
        if (exceptionType.Contains("ValidationException"))
            return "validation_error";

        // Timeouts y cancelaciones
        if (ex is TimeoutException)
            return "timeout_error";
        if (ex is OperationCanceledException)
            return "provider_error";

        // HTTP (proveedor)
        if (ex is HttpRequestException httpEx)
        {
            return httpEx.InnerException switch
            {
                TimeoutException => "timeout_error",
                _ => "provider_error"
            };
        }

        // Excepciones de dominio
        if (exceptionType.Contains("DomainException") || exceptionType.Contains("BusinessException"))
            return "business_error";

        return "unknown_error";
    }
}
