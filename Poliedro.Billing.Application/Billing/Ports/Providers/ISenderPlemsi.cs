using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Ports.Providers;

/// <summary>
/// Interfaz para manejar el envío de facturas a Plemsi.
/// Cada implementación maneja un tipo específico de factura (FE, POS, Credit Note, Debit Note)
/// y normaliza la respuesta de Plemsi.
/// </summary>
public interface ISenderPlemsi
{
    /// <summary>
    /// Envía facturas preparadas a Plemsi y normaliza la respuesta.
    /// </summary>
    /// <param name="invoices">Facturas ya preparadas en formato Plemsi</param>
    /// <param name="resolution">Resolución DIAN</param>
    /// <param name="company">Proveedor de facturación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado normalizado del envío</returns>
    Task<IEnumerable<BillingResultItem>> SendAndNormalizeAsync(
        IEnumerable<object> invoices,
        DianResolutionEntity resolution,
        CompanyProviderEntity company,
        CancellationToken cancellationToken);
}
