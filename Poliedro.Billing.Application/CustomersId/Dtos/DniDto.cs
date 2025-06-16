

namespace Poliedro.Billing.Application.CustomersId.Dtos
{
    public record DniDto(
    string Type,
    string Number,
    string VerificationDigit
);
}
