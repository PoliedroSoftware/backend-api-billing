namespace Poliedro.Billing.Application.BillingCreditNote.Dtos.Plemsi;
public record DiscrepancyDTO
{
    public int code { get; init; } // 2

    public string description { get; init; } //"Anulación solicitada por el emisor"
}
