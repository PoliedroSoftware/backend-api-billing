namespace Poliedro.Billing.Application.Billing.Dtos;

public record AllowanceChargeDTO
(
    bool ChargeIndicator,
    string? AllowanceChargeReason,
    decimal MultiplierFactorNumeric,
    decimal Amount,
    decimal BaseAmount
);