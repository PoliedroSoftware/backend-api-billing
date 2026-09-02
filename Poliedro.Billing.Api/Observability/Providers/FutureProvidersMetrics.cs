using Prometheus;

namespace Poliedro.Billing.Api.Observability.Providers;

/// <summary>
/// Implementación de métricas para el proveedor Siigo (Accounting).
/// 
/// Ubicación: Observability/Providers
/// Propósito: Separar métricas por proveedor para mantener escalabilidad.
/// 
/// Patrón: Si necesitas agregar Siigo, Factus, o cualquier otro proveedor:
/// 1. Copiar esta clase
/// 2. Cambiar clase a SiigoMetrics, ProviderName = "siigo"
/// 3. Crear decorators correspondientes en Decorators/
/// 4. Registrar en Program.cs:
///    services.AddSingleton<IProviderMetrics, SiigoMetrics>();
/// 
/// Beneficio: Cada proveedor tiene su propia instancia con sus propias métricas.
/// No hay interferencia entre providers.
/// </summary>
public class SiigoMetrics : IProviderMetrics
{
    public string ProviderName => "siigo";

    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter(
            "billing_siigo_requests_total",
            "Número total de solicitudes realizadas a Siigo",
            new CounterConfiguration { LabelNames = new[] { "request_type" } }
        );

    private static readonly Counter ErrorsTotal = Metrics
        .CreateCounter(
            "billing_siigo_errors_total",
            "Número total de errores reportados por Siigo",
            new CounterConfiguration { LabelNames = new[] { "request_type", "error_reason" } }
        );

    private static readonly Histogram DurationSeconds = Metrics
        .CreateHistogram(
            "billing_siigo_duration_seconds",
            "Tiempo de respuesta de Siigo en segundos",
            new HistogramConfiguration
            {
                LabelNames = new[] { "request_type" },
                Buckets = new[] { 0.5, 1.0, 2.0, 5.0, 10.0, 30.0 }
            }
        );

    private static readonly Gauge ProviderStatus = Metrics
        .CreateGauge(
            "billing_provider_status",
            "Estado del proveedor (1=disponible, 0=error)",
            new GaugeConfiguration { LabelNames = new[] { "provider" } }
        );

    public IProviderMetricsScope BeginRequest()
    {
        return new SiigoMetricsScope();
    }

    public void RecordSuccess(IProviderMetricsScope scope, string requestType)
    {
        if (scope is not SiigoMetricsScope siigoScope)
            return;

        RequestsTotal.WithLabels(requestType).Inc();
        DurationSeconds.WithLabels(requestType).Observe(DateTime.UtcNow.Subtract(siigoScope.StartTime).TotalSeconds);
        ProviderStatus.WithLabels(ProviderName).Set(1);
    }

    public void RecordError(IProviderMetricsScope scope, string requestType, string errorReason)
    {
        if (scope is not SiigoMetricsScope siigoScope)
            return;

        RequestsTotal.WithLabels(requestType).Inc();
        ErrorsTotal.WithLabels(requestType, errorReason).Inc();
        DurationSeconds.WithLabels(requestType).Observe(DateTime.UtcNow.Subtract(siigoScope.StartTime).TotalSeconds);
        ProviderStatus.WithLabels(ProviderName).Set(0);
    }

    public void SetProviderStatus(double status)
    {
        ProviderStatus.WithLabels(ProviderName).Set(status);
    }

    private class SiigoMetricsScope : IProviderMetricsScope
    {
        public DateTime StartTime { get; } = DateTime.UtcNow;
        public void Dispose() { }
    }
}

/// <summary>
/// Implementación de métricas para el proveedor TNS (Billing).
/// 
/// Misma estructura que SiigoMetrics pero para TNS.
/// </summary>
public class TnsMetrics : IProviderMetrics
{
    public string ProviderName => "tns";

    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter(
            "billing_tns_requests_total",
            "Número total de solicitudes realizadas a TNS",
            new CounterConfiguration { LabelNames = new[] { "request_type" } }
        );

    private static readonly Counter ErrorsTotal = Metrics
        .CreateCounter(
            "billing_tns_errors_total",
            "Número total de errores reportados por TNS",
            new CounterConfiguration { LabelNames = new[] { "request_type", "error_reason" } }
        );

    private static readonly Histogram DurationSeconds = Metrics
        .CreateHistogram(
            "billing_tns_duration_seconds",
            "Tiempo de respuesta de TNS en segundos",
            new HistogramConfiguration
            {
                LabelNames = new[] { "request_type" },
                Buckets = new[] { 0.5, 1.0, 2.0, 5.0, 10.0, 30.0 }
            }
        );

    private static readonly Gauge ProviderStatus = Metrics
        .CreateGauge(
            "billing_provider_status",
            "Estado del proveedor (1=disponible, 0=error)",
            new GaugeConfiguration { LabelNames = new[] { "provider" } }
        );

    public IProviderMetricsScope BeginRequest()
    {
        return new TnsMetricsScope();
    }

    public void RecordSuccess(IProviderMetricsScope scope, string requestType)
    {
        if (scope is not TnsMetricsScope tnsScope)
            return;

        RequestsTotal.WithLabels(requestType).Inc();
        DurationSeconds.WithLabels(requestType).Observe(DateTime.UtcNow.Subtract(tnsScope.StartTime).TotalSeconds);
        ProviderStatus.WithLabels(ProviderName).Set(1);
    }

    public void RecordError(IProviderMetricsScope scope, string requestType, string errorReason)
    {
        if (scope is not TnsMetricsScope tnsScope)
            return;

        RequestsTotal.WithLabels(requestType).Inc();
        ErrorsTotal.WithLabels(requestType, errorReason).Inc();
        DurationSeconds.WithLabels(requestType).Observe(DateTime.UtcNow.Subtract(tnsScope.StartTime).TotalSeconds);
        ProviderStatus.WithLabels(ProviderName).Set(0);
    }

    public void SetProviderStatus(double status)
    {
        ProviderStatus.WithLabels(ProviderName).Set(status);
    }

    private class TnsMetricsScope : IProviderMetricsScope
    {
        public DateTime StartTime { get; } = DateTime.UtcNow;
        public void Dispose() { }
    }
}
