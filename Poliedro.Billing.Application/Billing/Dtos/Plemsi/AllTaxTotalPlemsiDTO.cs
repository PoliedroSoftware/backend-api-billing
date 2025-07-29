namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record AllTaxTotalPlemsiDTO
    (
      int tax_id,
      double tax_amount,
      double percent,
      double taxable_amount
    );
