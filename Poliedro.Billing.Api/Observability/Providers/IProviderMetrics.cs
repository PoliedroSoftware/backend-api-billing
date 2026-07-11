namespace Poliedro.Billing.Api.Observability.Providers;

/// <summary>
/// Define el contrato para métricas de proveedores externos.
/// 
/// Ubicación: En API layer, pero con interfaz para permitir extensión limpia.
/// Propósito: Abstracción SOLID - permitir que nuevos proveedores registren sus propias métricas
/// sin modificar la instrumentación central.
/// 
/// Patrón: Strategy pattern - cada proveedor puede tener su propia implementación.
/// 
/// Futuros proveedores: Siigo, TNS, Factus, Carvajal, etc.
/// Solo necesitan:
/// 1. Crear clase que implemente esta interfaz
/// 2. Registrar en DI en Program.cs
/// 3. Inyectar en su adapter
/// </summary>
public interface IProviderMetrics
{
    /// <summary>
    /// Nombre único del proveedor (e.g., "plemsi", "siigo", "tns").
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Registra el inicio de una solicitud al proveedor.
    /// Retorna un token para medir la duración total.
    /// </summary>
    IProviderMetricsScope BeginRequest();

    /// <summary>
    /// Registra el éxito de una solicitud.
    /// Parámetros:
    /// - scope: token del BeginRequest
    /// - requestType: tipo de solicitud (e.g., "create_invoice", "get_invoice", "credit_note")
    /// </summary>
    void RecordSuccess(IProviderMetricsScope scope, string requestType);

    /// <summary>
    /// Registra el error de una solicitud.
    /// Parámetros:
    /// - scope: token del BeginRequest
    /// - requestType: tipo de solicitud
    /// - errorReason: categoría del error (e.g., "timeout", "invalid_response", "authentication_failed")
    /// </summary>
    void RecordError(IProviderMetricsScope scope, string requestType, string errorReason);

    /// <summary>
    /// Actualiza el estado del proveedor.
    /// 1 = disponible, 0 = con error/indisponible.
    /// </summary>
    void SetProviderStatus(double status);
}

/// <summary>
/// Representa un scope de medición para una solicitud al proveedor.
/// Permite pasar contexto entre BeginRequest y RecordSuccess/RecordError.
/// </summary>
public interface IProviderMetricsScope : IDisposable
{
    /// <summary>
    /// Timestamp del inicio de la solicitud.
    /// </summary>
    DateTime StartTime { get; }
}
