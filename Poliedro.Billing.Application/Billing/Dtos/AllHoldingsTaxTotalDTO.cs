namespace Poliedro.Billing.Application.Billing.Dtos;

public record AllHoldingsTaxTotalDTO
    (   
        int TaxId,
        double TaxAmount,
        double Percent,
        double TaxableAmount
    );