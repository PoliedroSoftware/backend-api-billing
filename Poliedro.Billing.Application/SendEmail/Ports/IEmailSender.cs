using Poliedro.Billing.Application.SendEmail.Dtos;
namespace Poliedro.Billing.Domain.SendEmail.Ports;
public interface IEmailSender
{
    Task SendEmailAsync(EmailMessageDto message);
}
