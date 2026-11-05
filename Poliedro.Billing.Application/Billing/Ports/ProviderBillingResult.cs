namespace Poliedro.Billing.Application.Billing.Ports;

public class ProviderBillingResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public IEnumerable<BillingResultItem> Results { get; init; } = [];
}
