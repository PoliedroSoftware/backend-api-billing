namespace Poliedro.Billing.Application.Billing.Dtos;

public record PayPointInfoDTO
(
    string Code,
    string Address,
    string CashierName,
    string PayPointType,
    string SaleCode
);