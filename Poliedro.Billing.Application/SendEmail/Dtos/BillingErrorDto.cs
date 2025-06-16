namespace Poliedro.Billing.Application.SendEmail.Dtos;
public record BillingErrorDto(
    string? Prefix,
    string? Number,
    string? Name,
    string? ErrorMessage
    );
