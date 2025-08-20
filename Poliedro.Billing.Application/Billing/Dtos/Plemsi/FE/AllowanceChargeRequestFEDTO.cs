namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record AllowanceChargeRequestFEDTO
{
    public bool charge_indicator {  get; init; }
    public string? allowance_charge_reason { get; init; }
    public decimal multiplier_factor_numeric { get; init; }
    public decimal amount {  get; init; }
    public decimal base_amount { get; init; }
}