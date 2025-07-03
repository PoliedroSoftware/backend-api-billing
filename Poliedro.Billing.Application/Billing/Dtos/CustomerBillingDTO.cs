namespace Poliedro.Billing.Application.Billing.Dtos;

public record CustomerBillingDTO
    (
        string IdentificationNumber,
        string? Dv,
        int? Profit,
        string Name,
        string? Phone,
        string Address,
        string Email,
        string? MerchantRegistration,
        int TypeDocumentIdentificationId,
        int? TypeOrganizationId,
        int TypeLiabilityId,
        int? MunicipalityId,
        int TypeRegimeId
    );