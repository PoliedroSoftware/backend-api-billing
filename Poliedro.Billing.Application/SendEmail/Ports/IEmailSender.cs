using Poliedro.Billing.Application.SendEmail.Dtos;
namespace Poliedro.Billing.Application.SendEmail.Ports;
public interface IEmailSender
{
    Task SendEmailAsync(EmailMessageDto message);
}
