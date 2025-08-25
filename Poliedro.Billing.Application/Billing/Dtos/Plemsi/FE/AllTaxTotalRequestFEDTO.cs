namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record AllTaxTotalRequestFEDTO
{
      public int tax_id { get; init; }
      public int tax_amount { get; init; }
      public int percent {  get; init; }
      public int taxable_amount { get; init; }
    }
