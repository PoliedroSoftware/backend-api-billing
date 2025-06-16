namespace Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote
{
    public record Seller(
        string IdentificationNumber,
        string Dv,
        string Name,
        string Phone,
        string Address,
        string PostalZoneCode,
        string Email,
        string MerchantRegistration,
        int TypeDocumentIdentificationId,
        int TypeOrganizationId,
        int TypeLiabilityId,
        int MunicipalityId,
        int TypeRegimeId
    );
}
