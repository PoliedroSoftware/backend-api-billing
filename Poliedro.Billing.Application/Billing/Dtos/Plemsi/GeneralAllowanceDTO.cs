namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record GeneralAllowanceDTO
    (
     string? allowance_charge_reason,
     double? allowance_percent,
     decimal amount,
     decimal base_amount
    );