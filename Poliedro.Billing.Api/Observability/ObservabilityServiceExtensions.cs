using Prometheus;
using Poliedro.Billing.Api.Observability.Pipeline;
using Poliedro.Billing.Api.Observability.Providers;
using MediatR;

namespace Poliedro.Billing.Api.Observability;

/// <summary>
/// Extensión de IServiceCollection para registrar servicios de observabilidad.
/// 
/// Ubicación: Centraliza toda la configuración de Prometheus en UN lugar.
/// Propósito: Program.cs queda limpio, solo llamar .AddObservability().
/// 
/// Responsabilidad:
/// - Registrar metrics de prometheus-net
/// - Configurar middleware HTTP
/// - Registrar Pipeline Behaviors de MediatR
/// - Registrar Decorators de observabilidad
/// 
/// Ventajas:
/// ✓ Program.cs limpio y legible
/// ✓ Si necesitas cambiar observabilidad, tienes 1 lugar
/// ✓ Fácil de testear (inyectar mock de IProviderMetrics)
/// ✓ Reutilizable en otros proyectos
/// </summary>
public static class ObservabilityServiceExtensions
{
    /// <summary>
    /// Registra todos los servicios de observabilidad.
    /// 
    /// Incluye:
    /// - Métricas de Prometheus
    /// - Pipeline Behaviors de MediatR
    /// - Provider metrics (Plemsi, etc)
    /// 
    /// Uso en Program.cs:
    /// builder.Services.AddObservability();
    /// </summary>
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        // Registrar IProviderMetrics para Plemsi
        services.AddSingleton<IProviderMetrics, PlemsiMetrics>();

        // Registrar Pipeline Behavior para instrumentación de handlers
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(MetricsObservabilityBehavior<,>)
        );

        return services;
    }

    /// <summary>
    /// Configura el middleware HTTP de prometheus-net.
    /// 
    /// Expone:
    /// - /metrics → endpoint Prometheus scrape
    /// - Métricas automáticas: request count, latencia, status code
    /// 
    /// Uso en Program.cs (después de app = builder.Build()):
    /// app.UsePrometheusMetrics();
    /// app.MapMetrics();
    /// 
    /// Nota: mapMetrics() mapea el endpoint /metrics explícitamente.
    /// </summary>
    public static WebApplication UsePrometheusMetrics(this WebApplication app)
    {
        // Middleware HTTP que captura todas las requests
        app.UseHttpMetrics();

        return app;
    }

    /// <summary>
    /// Mapea el endpoint /metrics para Prometheus scrape.
    /// 
    /// Llamar DESPUÉS de UsePrometheusMetrics().
    /// </summary>
    public static WebApplication MapPrometheusMetrics(this WebApplication app)
    {
        // Expone métricas en /metrics usando la extensión de prometheus-net
        app.MapMetrics();

        return app;
    }
}
