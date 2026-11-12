using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Ports.Providers;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Infraestructure.External.Provider.Provider.Plemsi;

/// <summary>
/// Obtiene el siguiente consecutivo de Plemsi para una resolución.
///
/// Hoy la consulta se hace una sola vez por lote y luego se avanza en memoria:
/// si una factura falla se deja el hueco (opción A). Cuando se pase a consultar
/// por factura, solo cambia el interior de <see cref="NextAsync"/>; el llamador
/// ya pide el consecutivo justo antes de cada envío.
/// </summary>
public class PlemsiConsecutiveProvider(
    IGetLastInvoiceBilling lastInvoiceBilling,
    ILogger<PlemsiConsecutiveProvider> logger) : IConsecutiveProvider
{
    private int? _next;

    public async Task<ConsecutiveResult> NextAsync(
        DianResolutionEntity resolution,
        CompanyProviderEntity companyProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            if (_next is null)
            {
                var lastEmitted = await lastInvoiceBilling.GetLastEmittedNumberAsync(
                    resolution, companyProvider, cancellationToken);

                if (resolution.ExpirationDate != default && DateTime.Now > resolution.ExpirationDate)
                {
                    logger.LogWarning(
                        "La resolución {ResolutionId} está vencida desde {ExpirationDate}.",
                        resolution.ResolutionId, resolution.ExpirationDate);

                    return ConsecutiveResult.Fail(
                        "resolution_expired", "La resolución está vencida.");
                }

                // Si no hay documentos emitidos todavía, el primero es el inicio del rango autorizado.
                var next = Math.Max(lastEmitted + 1, resolution.InitialRange);

                if (next > resolution.FinalRange)
                {
                    logger.LogWarning(
                        "El rango de la resolución {ResolutionId} está agotado (último {LastEmitted}, final {FinalRange}).",
                        resolution.ResolutionId, lastEmitted, resolution.FinalRange);

                    return ConsecutiveResult.Fail(
                        "range_exhausted", "El rango de numeración de la resolución está agotado.");
                }

                _next = next;
            }

            var number = _next.Value;
            _next++;

            return ConsecutiveResult.Ok(number);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "No fue posible obtener el consecutivo de Plemsi para la resolución {ResolutionId}.",
                resolution.ResolutionId);

            return ConsecutiveResult.Fail(
                "consecutive_unavailable",
                "No fue posible obtener el consecutivo desde el proveedor.");
        }
    }
}
