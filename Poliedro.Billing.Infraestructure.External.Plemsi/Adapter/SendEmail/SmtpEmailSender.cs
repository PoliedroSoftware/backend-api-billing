
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Application.SendEmail.Ports;
using Poliedro.Billing.Domain.SendEmail.Ports;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;

public class SmtpEmailSender(IConfiguration _config, IEmailBodyRenderer _renderer) : IEmailSender
{

    public async Task SendEmailAsync(EmailMessageDto message)
    {
        var username = _config["EmailSettings:Username"];
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentNullException(nameof(username), "Email username cannot be null.");
        }

        var smtpClient = new SmtpClient(_config["EmailSettings:Protocol"])
        {
            Port = Convert.ToInt32(_config["EmailSettings:Port"]),
            Credentials = new NetworkCredential(
                username,
                _config["EmailSettings:AppPassword"]),
            EnableSsl = true
        };

        var bodyHtml = _renderer.Render(message.Body!);
        try
        {
           
            var mail = new MailMessage(
           from: username,
           to: message.To!,
           subject: message.Subject ?? string.Empty,
           body: bodyHtml
       )
            {
                IsBodyHtml = true
            };
            await smtpClient.SendMailAsync(mail);
        }
        catch (Exception ex)
        {

            throw ex;
        }

      
       
    }
}
