namespace Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.FE;
public record AllHoldingsTaxTotalRequestFEDTO
{
     public int tax_id { get; init; }
     public double tax_amount { get; init; }
     public double percent {  get; init; }
     public double taxable_amount { get; init; }
    }