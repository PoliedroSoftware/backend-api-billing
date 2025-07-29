namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record WIthHoldingTaxTotalPlemsiDTO
    (
    int tax_id,
    int percent,
    int tax_amount,
    int taxable_amount
    );
