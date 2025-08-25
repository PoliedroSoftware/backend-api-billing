namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.POS;

public record TaxTotalRequestPosDto
{
    public int tax_id { get; init; }
    public int tax_amount { get; init; }
    public int percent { get; init; }
    public decimal taxable_amount { get; init; }
}
