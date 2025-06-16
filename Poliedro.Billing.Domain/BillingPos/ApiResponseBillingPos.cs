namespace Poliedro.Billing.Domain.BillingPos;

public class ApiResponseBillingPos
{
    public int Code { get; init; }
    public bool Success { get; init; }
    public string Info { get; init; }
    public ApiDataPos Data { get; init; }
}
