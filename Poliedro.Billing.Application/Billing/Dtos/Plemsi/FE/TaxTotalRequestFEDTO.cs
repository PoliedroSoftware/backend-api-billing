namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record TaxTotalRequestFEDTO
{
    public int tax_id { get; init; }
    public double percent { get; init; }
    public double tax_amount { get; init; }
    public double taxable_amount { get; init; }
    }