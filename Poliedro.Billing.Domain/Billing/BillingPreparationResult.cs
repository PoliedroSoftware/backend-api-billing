namespace Poliedro.Billing.Domain.Billing;

/// <summary>
/// Resultado de preparar una factura para el proveedor. Cuando <see cref="Error"/>
/// tiene valor, la factura no debe enviarse y el motivo se reporta al cliente.
/// </summary>
public sealed record BillingPreparationResult(CreateBilling Billing, object? Output, string? Error);
