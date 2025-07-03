namespace Poliedro.Billing.Application.Billing.Dtos;

public record WIthHoldingTaxTotalDTO
(
    int TaxId,
    int Percent,
    int TaxAmount,
    int TaxableAmount
);