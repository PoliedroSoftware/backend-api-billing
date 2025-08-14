
namespace Poliedro.Billing.Application.Billing.Dtos;

public record GeneralAllowanceDTO
(
    string? AllowanceChargeReason,
    double? AllowancePercent,
    decimal Amount,
    decimal BaseAmount
);