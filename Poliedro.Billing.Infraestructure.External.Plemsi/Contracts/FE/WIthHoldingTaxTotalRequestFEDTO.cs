namespace Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.FE;
public record WIthHoldingTaxTotalRequestFEDTO
{
    public int tax_id { get; init; }
    public int percent {  get; init; }
    public int tax_amount { get; init; }
    public int taxable_amount { get; init; }
    }
