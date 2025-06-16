using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Application.Helper.EmailBuilder;
public static class EmailMessageBuilder
{
    public static EmailMessageDto BuildResolutionExpiredMessage(ClientEntity clientItem, int invoice,
        string companyEmail, string subject = "Resolución Expirada")
    {
        var to = $"{clientItem.Email},{companyEmail}";

        var reason = invoice > clientItem.DianResolution.FinalRange
            ? "por numeración"
            : "por fecha";

        return new EmailMessageDto(
         To: to,
         Subject: subject,
         Body: new EmailBodyDto(
             Greeting: "Estimado cliente:",
             Message: $"La resolución {reason} ha expirado.",
             Footer: "Gracias por su atención.",
             AdditionalData: new Dictionary<string, string>
             {
                    { "Cliente", clientItem.Name },
                    { "Fecha", clientItem.DianResolution.ExpirationDate.ToString() },
                    { "Resolución", clientItem.DianResolution.ResolutionNumber },
                    { "Rango Inicial", clientItem.DianResolution.InitialRange.ToString() },
                    { "Rango Final", clientItem.DianResolution.FinalRange.ToString() },
                    { "Tipo de Resolución", clientItem.DianResolution.Description },
                    { "Numeración Actual", clientItem.DianResolution.CurrentlyNumber.ToString() },
                    { "Última Factura", invoice.ToString() }
             }
         )
         );

    }
}