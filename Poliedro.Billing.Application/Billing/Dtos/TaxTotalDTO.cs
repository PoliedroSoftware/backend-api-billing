namespace Poliedro.Billing.Application.Billing.Dtos;

public record TaxTotalDTO
(
    int TaxId,
    double Percent,
    double TaxAmount,
    double TaxableAmount
);