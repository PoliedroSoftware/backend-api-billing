namespace Poliedro.Billing.Application.Billing.Dtos;

public record CustomerBillingDTO
    (
        string IdentificationNumber,
        string? MultipleResolution,
        string? ApiKey,
        string? Dv,
        int? Profit,
        string Name,
        string? Phone,
        string? Address,
        string Email,
        string? MerchantRegistration,
        string? City,
        string? State,
        string? Country,
        int? TypeDocumentIdentificationId,
        int? TypeOrganizationId,
        int? TypeLiabilityId,
        int? MunicipalityId,
        string? MunicipalityCode,
        int? TypeRegimeId
    );