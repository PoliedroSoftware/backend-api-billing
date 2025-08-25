namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi.FE;
public record PaymentRequestFEDTO
{
     public int payment_form_id {  get; init; }
     public int payment_method_id { get; init; }
     public string? payment_due_date { get; init; }
     public string? duration_measure { get; init; }
}