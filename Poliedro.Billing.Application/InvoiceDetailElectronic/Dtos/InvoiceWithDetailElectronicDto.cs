namespace Poliedro.Billing.Application.InvoiceDetailElectronic.Dtos;

public record InvoiceWithDetailElectronicDto(
    int Id,
    int Transaccion,
    int Code,
    int TypeItemIdentification,
    string? Description,
    int UnitMeasureId,
    decimal BaseQuantity,
    decimal InvoicedQuantity,
    decimal? PriceAmount,
    decimal? LineExtensionAmount,
    double? Percent,
    decimal TaxAmount,
    decimal? UnitPrice,
    string? Invoice,
    DateTime? DateRegister,
    string? Verify
);