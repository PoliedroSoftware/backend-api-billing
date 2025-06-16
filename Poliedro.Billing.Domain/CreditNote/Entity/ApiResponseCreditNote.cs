namespace Poliedro.Billing.Domain.CreditNote.Entity;

public class ApiResponseCreditNote
{
    public int Code { get; init; }
    public bool Success { get; init; }
    public string Info { get; init; }
    public dynamic Data { get; init; }
}