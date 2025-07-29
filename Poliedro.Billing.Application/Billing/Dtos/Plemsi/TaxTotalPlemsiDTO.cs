namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record TaxTotalPlemsiDTO
    (
    int tax_id,
    double percent,
    double tax_amount,
    double taxable_amount
    );