using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Billing.Ports.Providers;

/// <summary>
/// Obtiene el siguiente consecutivo fiscal de una resolución para el proveedor.
/// La forma de obtenerlo es una condición propia de cada proveedor (Plemsi lo
/// expone consultando sus documentos emitidos), por eso la implementación vive
/// en la capa externa del proveedor y no en la preparación de la factura.
/// </summary>
public interface IConsecutiveProvider
{
    Task<ConsecutiveResult> NextAsync(
        DianResolutionEntity resolution,
        CompanyProviderEntity companyProvider,
        CancellationToken cancellationToken);
}

/// <summary>
/// Resultado de pedir un consecutivo. <see cref="ErrorCode"/> permite distinguir
/// el motivo sin depender del texto que se muestra al cliente.
/// </summary>
public sealed record ConsecutiveResult(bool Success, int Number, string? ErrorCode, string? Message)
{
    public static ConsecutiveResult Ok(int number) => new(true, number, null, null);

    public static ConsecutiveResult Fail(string errorCode, string message) => new(false, 0, errorCode, message);
}
