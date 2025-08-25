namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record CustomSubtotalRequestFEDTO
{
    public string? concept {  get; init; }
    public double amount { get; init; }
    }