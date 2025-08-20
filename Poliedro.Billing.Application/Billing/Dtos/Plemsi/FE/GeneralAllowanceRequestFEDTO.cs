namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record GeneralAllowanceRequestFEDTO
{
     public string? allowance_charge_reason {  get; init; }
     public double? allowance_percent { get; init; }
     public decimal amount { get; init; }
     public decimal base_amount { get; init; }
}