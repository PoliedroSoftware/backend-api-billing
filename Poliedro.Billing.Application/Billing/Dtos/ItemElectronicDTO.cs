namespace Poliedro.Billing.Application.Billing.Dtos;

public record ItemElectronicDTO
    (
        int UnitMeasureId,
        double LineExtensionAmount,
        bool FreeOfChargeIndicator,
        List<AllowanceChargeDTO>? AllowanceCharges,
        List<TaxTotalDTO>? TaxTotals,
        List<WIthHoldingTaxTotalDTO>? WithHoldingTaxTotal,
        string? Description,
        string? Notes,
        string? Code,
        int TypeItemIdentificationId,
        double PriceAmount,
        double BaseQuantity,
        double InvoicedQuantity
    );