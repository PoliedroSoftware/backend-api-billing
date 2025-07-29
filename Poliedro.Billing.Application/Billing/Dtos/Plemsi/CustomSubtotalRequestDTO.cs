namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record CustomSubtotalRequestDTO
{
    public string? concept {  get; init; }
    public double amount { get; init; }
    }