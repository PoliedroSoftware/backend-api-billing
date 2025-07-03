namespace Poliedro.Billing.Application.Billing.Dtos;

public record PaymentDTO(
    int PaymentFormId,
    int PaymentMethodId,
    string? PaymentDueDate,
    string? DurationMeasure
);