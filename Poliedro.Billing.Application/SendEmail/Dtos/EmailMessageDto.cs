namespace Poliedro.Billing.Application.SendEmail.Dtos;
public record EmailMessageDto(
    string? To,
    string? Subject,
    EmailBodyDto? Body
);