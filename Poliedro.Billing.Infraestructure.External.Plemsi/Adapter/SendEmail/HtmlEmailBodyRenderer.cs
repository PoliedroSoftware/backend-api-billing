using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Application.SendEmail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;

public class HtmlEmailBodyRenderer : IEmailBodyRenderer
{
    public string Render(EmailBodyDto body)
    {
        var html = $"{body.Greeting}<br><br>{body.Message}<br><br>";

        if (body.AdditionalData != null)
        {
            foreach (var item in body.AdditionalData)
            {
                html += $"<strong>{item.Key}:</strong> {item.Value}<br>";
            }
        }

        html += $"<br>{body.Footer}";
        return html;
    }
}

