namespace Poliedro.Billing.Application.SendEmail.Dtos;
public record EmailBodyDto(
    string? Greeting,
    string? Message,
    string? Footer,
    Dictionary<string, string>? AdditionalData
    );




