using Prometheus;

namespace Poliedro.Billing.Api.Observability.Providers;

/// <summary>
/// Implementación de métricas para el proveedor Plemsi.
/// 
/// Ubicación: Dedica sección en Observability para cada proveedor.
/// Responsabilidad: Encapsular TODAS las métricas de Plemsi en un solo lugar.
/// 
/// Propósito: Cuando agregues un nuevo proveedor (Siigo, TNS, etc.),
/// simplemente crear una clase similar e inyectar en el adapter correspondiente.
/// 
/// Beneficio: La lógica de instrumentación está separada de la lógica de negocio.
/// </summary>
public class PlemsiMetrics : IProviderMetrics
{
    public string ProviderName => "plemsi";

    /// <summary>
    /// Contador de solicitudes totales realizadas a Plemsi.
    /// Incrementa con cada HTTP call (éxito o error).
    /// 
    /// Labels: request_type (create_invoice, get_invoice, credit_note, etc.)
    /// </summary>
    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter(
            "billing_plemsi_requests_total",
            "Número total de solicitudes realizadas a Plemsi",
            new CounterConfiguration
            {
                LabelNames = new[] { "request_type" }
            }
        );

    /// <summary>
    /// Contador de errores reportados por Plemsi.
    /// Incrementa cuando la respuesta indica error en el proveedor (5xx, timeout, malformed response, etc).
    /// 
    /// Labels:
    /// - request_type: tipo de solicitud
    /// - error_reason: categoría del error (timeout, invalid_response, auth_failed, business_error)
    /// </summary>
    private static readonly Counter ErrorsTotal = Metrics
        .CreateCounter(
            "billing_plemsi_errors_total",
            "Número total de errores reportados por Plemsi",
            new CounterConfiguration
            {
                LabelNames = new[] { "request_type", "error_reason" }
            }
        );

    /// <summary>
    /// Histograma de tiempo de respuesta de Plemsi.
    /// Mide latencia end-to-end: desde HTTP call hasta recibir respuesta.
    /// 
    /// Labels: request_type
    /// Buckets: 500ms, 1s, 2s, 5s, 10s, 30s (proveedores externos pueden ser lentos)
    /// </summary>
    private static readonly Histogram DurationSeconds = Metrics
        .CreateHistogram(
            "billing_plemsi_duration_seconds",
            "Tiempo de respuesta de Plemsi en segundos",
            new HistogramConfiguration
            {
                LabelNames = new[] { "request_type" },
                Buckets = new[] { 0.5, 1.0, 2.0, 5.0, 10.0, 30.0 }
            }
        );

    /// <summary>
    /// Gauge del estado del proveedor Plemsi.
    /// 1 = disponible/saludable
    /// 0 = con error/indisponible
    /// 
    /// Se actualiza tras cada interacción.
    /// Si hay errores consecutivos → 0.
    /// Si tenemos éxito → 1.
    /// </summary>
    private static readonly Gauge ProviderStatus = Metrics
        .CreateGauge(
            "billing_provider_status",
            "Estado del proveedor (1=disponible, 0=error)",
            new GaugeConfiguration
            {
                LabelNames = new[] { "provider" }
            }
        );

    public IProviderMetricsScope BeginRequest()
    {
        return new PlemsiMetricsScope();
    }

    public void RecordSuccess(IProviderMetricsScope scope, string requestType)
    {
        if (scope is not PlemsiMetricsScope plemsiScope)
            return;

        RequestsTotal.WithLabels(requestType).Inc();
        DurationSeconds.WithLabels(requestType).Observe(DateTime.UtcNow.Subtract(plemsiScope.StartTime).TotalSeconds);
        ProviderStatus.WithLabels(ProviderName).Set(1);
    }

    public void RecordError(IProviderMetricsScope scope, string requestType, string errorReason)
    {
        if (scope is not PlemsiMetricsScope plemsiScope)
            return;

        RequestsTotal.WithLabels(requestType).Inc();
        ErrorsTotal.WithLabels(requestType, errorReason).Inc();
        DurationSeconds.WithLabels(requestType).Observe(DateTime.UtcNow.Subtract(plemsiScope.StartTime).TotalSeconds);
        ProviderStatus.WithLabels(ProviderName).Set(0);
    }

    public void SetProviderStatus(double status)
    {
        ProviderStatus.WithLabels(ProviderName).Set(status);
    }

    /// <summary>
    /// Scope interno para medir duración de solicitudes.
    /// </summary>
    private class PlemsiMetricsScope : IProviderMetricsScope
    {
        public DateTime StartTime { get; } = DateTime.UtcNow;

        public void Dispose()
        {
            // No necesita limpieza en este caso.
        }
    }
}
