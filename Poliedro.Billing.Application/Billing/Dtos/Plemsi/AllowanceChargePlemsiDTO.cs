namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record AllowanceChargePlemsiDTO
    (
    bool charge_indicator,
    string? allowance_charge_reason,
    decimal multiplier_factor_numeric,
    decimal amount,
    decimal base_amount
    );