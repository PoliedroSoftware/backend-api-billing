namespace Poliedro.Billing.Application.Billing.Dtos;

public record AllTaxTotalDTO
(
    int TaxId,
    double TaxAmount,
    double Percent,
    double TaxableAmount
);