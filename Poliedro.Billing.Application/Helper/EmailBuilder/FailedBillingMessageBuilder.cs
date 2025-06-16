using Poliedro.Billing.Application.SendEmail.Dtos;
using System.Text;
namespace Poliedro.Billing.Application.Helper.EmailBuilder;
public static class FailedBillingMessageBuilder
{
    public static EmailMessageDto MessageBuilder(List<BillingErrorDto> errorBilling, string companyEmail)
    {
        var to = companyEmail;
        var subject = "Facturas con error en el envío a la DIAN";

        var uniqueErrors = errorBilling
          .GroupBy(e => $"{e.Prefix}{e.Number}{e.Name}")
          .Select(g => g.First())
          .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("Se encontraron las siguientes facturas con error en el envío a la DIAN:<br><br>");

        foreach (var billing in uniqueErrors)
        {
            sb.AppendLine($"- Factura: {billing.Prefix}{billing.Number}<br>");
            sb.AppendLine($"&nbsp;&nbsp;Cliente: {billing.Name}<br><br>");
            if (!string.IsNullOrWhiteSpace(billing.ErrorMessage))
            {
                sb.AppendLine($"&nbsp;&nbsp;Error: {billing.ErrorMessage}<br>");
            }

            sb.AppendLine("<br>");
        }

        sb.AppendLine($"Total: {uniqueErrors.Count} factura(s) con error.<br>");


        var body = new EmailBodyDto(
            Greeting: "Estimado usuario,",
            Message: sb.ToString(),
            Footer: "Este es un mensaje automático. Por favor no responda.",
            AdditionalData: null
        );

        return new EmailMessageDto(to, subject, body);
    }
}
