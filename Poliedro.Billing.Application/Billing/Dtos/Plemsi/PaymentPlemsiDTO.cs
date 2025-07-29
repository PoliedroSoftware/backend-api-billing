namespace Poliedro.Billing.Application.Billing.Dtos.Plemsi;
public record PaymentPlemsiDTO
    (
     int payment_form_id,
     int payment_method_id,
     string? payment_due_date,
     string? duration_measure
    );