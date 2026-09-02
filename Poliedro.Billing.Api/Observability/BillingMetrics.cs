using Prometheus;

namespace Poliedro.Billing.Api.Observability;

/// <summary>
/// Centraliza todas las métricas de negocio de facturación.
/// 
/// Ubicación: Archivo de definición centralizado en API layer (accesible por todos los handlers).
/// Propósito: Single Responsibility - definir y mantener todas las métricas de facturación en un solo lugar.
/// 
/// Patrón: Static para permitir acceso desde cualquier contexto (Metrics singleton de prometheus-net).
/// </summary>
public static class BillingMetrics
{
    /// <summary>
    /// Contador de facturas emitidas exitosamente.
    /// Incrementa únicamente cuando una factura es emitida exitosamente en el proveedor (Plemsi/Siigo/TNS).
    /// 
    /// Labels:
    /// - provider: "plemsi", "siigo", "tns"
    /// - client_id: ID del cliente
    /// - invoice_type: "FE" (factura electrónica), "POS" (punto de venta), "CN" (nota crédito)
    /// 
    /// Relevancia: KPI de negocio - mide productividad y éxito operacional.
    /// </summary>
    public static readonly Counter InvoiceSuccessTotal = Metrics
        .CreateCounter(
            "billing_invoice_success_total",
            "Número total de facturas emitidas exitosamente",
            new CounterConfiguration
            {
                LabelNames = new[] { "provider", "client_id", "invoice_type" }
            }
        );

    /// <summary>
    /// Contador de facturas que fallaron en la emisión.
    /// Incrementa cuando la emisión falla en cualquier etapa: validación, construcción, envío al proveedor.
    /// 
    /// Labels:
    /// - provider: "plemsi", "siigo", "tns"
    /// - client_id: ID del cliente
    /// - invoice_type: "FE", "POS", "CN"
    /// - error_reason: Categoría del error (e.g., "validation_error", "provider_error", "timeout", "invalid_data")
    /// 
    /// Relevancia: KPI de negocio - mide calidad y problemas operacionales.
    /// </summary>
    public static readonly Counter InvoiceFailedTotal = Metrics
        .CreateCounter(
            "billing_invoice_failed_total",
            "Número total de facturas que fallaron en la emisión",
            new CounterConfiguration
            {
                LabelNames = new[] { "provider", "client_id", "invoice_type", "error_reason" }
            }
        );

    /// <summary>
    /// Histograma del tiempo total de emisión de facturas.
    /// 
    /// Mide: Desde que inicia el handler CreateBillingHandler hasta que termina y devuelve el resultado.
    /// Incluye: validación, construcción de la factura, envío al proveedor, procesamiento de respuesta.
    /// 
    /// Labels:
    /// - provider: "plemsi", "siigo", "tns"
    /// - client_id: ID del cliente
    /// - invoice_type: "FE", "POS", "CN"
    /// 
    /// Buckets: 100ms, 500ms, 1s, 2s, 5s, 10s (captura latencias de servicios externos)
    /// 
    /// Relevancia: SLO/SLA - mide rendimiento y capacidad del sistema.
    /// </summary>
    public static readonly Histogram InvoiceDurationSeconds = Metrics
        .CreateHistogram(
            "billing_invoice_duration_seconds",
            "Tiempo total de emisión de una factura en segundos",
            new HistogramConfiguration
            {
                LabelNames = new[] { "provider", "client_id", "invoice_type" },
                Buckets = new[] { 0.1, 0.5, 1.0, 2.0, 5.0, 10.0 }
            }
        );
}
