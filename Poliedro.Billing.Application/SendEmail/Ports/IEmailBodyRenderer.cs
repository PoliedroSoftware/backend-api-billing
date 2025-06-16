using Poliedro.Billing.Application.SendEmail.Dtos;

namespace Poliedro.Billing.Application.SendEmail.Ports;
public interface IEmailBodyRenderer
{
    string Render(EmailBodyDto body);
}
