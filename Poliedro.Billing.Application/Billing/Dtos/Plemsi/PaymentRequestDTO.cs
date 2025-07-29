namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record PaymentRequestDTO
{
     public int payment_form_id {  get; init; }
     public int payment_method_id { get; init; }
     public string? payment_due_date { get; init; }
     public string? duration_measure { get; init; }
}