using Poliedro.Billing.Application.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Ports.Providers.Plemsi;

/// <summary>
/// Interfaz para manejar el envío de una factura a Plemsi.
/// Cada implementación maneja un tipo específico de factura (FE, POS, Credit Note, Debit Note)
/// y normaliza la respuesta de Plemsi.
/// </summary>
public interface ISenderPlemsi
{
    /// <summary>
    /// Envía una factura ya preparada a Plemsi y normaliza la respuesta.
    /// Se envía de a una factura para que el llamador pueda obtener y asignar
    /// el consecutivo del proveedor justo antes de cada envío.
    /// </summary>
    /// <param name="invoice">Factura preparada en formato Plemsi, con su consecutivo asignado</param>
    /// <param name="resolution">Resolución DIAN</param>
    /// <param name="company">Proveedor de facturación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado normalizado del envío</returns>
    Task<BillingResultItem> SendAsync(
        object invoice,
        DianResolutionEntity resolution,
        CompanyProviderEntity company,
        CancellationToken cancellationToken);
}
