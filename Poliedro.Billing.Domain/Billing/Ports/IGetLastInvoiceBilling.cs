using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IGetLastInvoiceBilling
{
    /// <summary>
    /// Devuelve el último número emitido en el proveedor para el prefijo de la resolución.
    /// Si aún no hay documentos emitidos devuelve 0.
    /// </summary>
    Task<int> GetLastEmittedNumberAsync(DianResolutionEntity dianResolutionEntity, CompanyProviderEntity companyProviderEntity, CancellationToken cancellationToken);
}
