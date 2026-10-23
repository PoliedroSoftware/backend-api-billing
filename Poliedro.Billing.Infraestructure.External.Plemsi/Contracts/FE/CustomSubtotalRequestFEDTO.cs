namespace Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.FE;
public record CustomSubtotalRequestFEDTO
{
    public string? concept {  get; init; }
    public double amount { get; init; }
    }