using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Application.Helper.EmailBuilder;
public static class EmailMessageBuilder
{
    public static EmailMessageDto BuildResolutionExpiredMessage(ClientEntity clientEntity, DianResolutionEntity dianResolutionEntity, int invoice,
        string companyEmail, string subject = "Resolución Expirada")
    {
        var to = $"{clientEntity.Email},{companyEmail}";

        var reason = invoice > dianResolutionEntity.FinalRange
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
                    { "Cliente", clientEntity.Name },
                    { "Fecha", dianResolutionEntity.ExpirationDate.ToString() },
                    { "Resolución", dianResolutionEntity.ResolutionNumber },
                    { "Rango Inicial", dianResolutionEntity.InitialRange.ToString() },
                    { "Rango Final", dianResolutionEntity.FinalRange.ToString() },
                    { "Tipo de Resolución", dianResolutionEntity.Description },
                    { "Numeración Actual", dianResolutionEntity.CurrentRange.ToString() },
                    { "Última Factura", invoice.ToString() }
             }
         )
         );

    }
}