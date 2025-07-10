using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Application.SendEmail.Ports;
using Poliedro.Billing.Domain.SendEmail.Ports;
using Microsoft.Extensions.Logging;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;

public class SmtpEmailSender(IConfiguration _config,
                             IEmailBodyRenderer _renderer,
                             ILogger<SmtpEmailSender> _logger) : IEmailSender
{
    public async Task SendEmailAsync(EmailMessageDto message)
    {
        var username = _config["EmailSettings:Username"];
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogError("Email username cannot be null or empty.");
            return;
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
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "Error enviando correo a {To}. Detalle: {Message}", message.To, ex.Message);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado enviando correo a {To}. Detalle: {Message}", message.To, ex.Message);

        }
    }
}
