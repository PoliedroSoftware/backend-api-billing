using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.DianResolution.Dtos;

public record DianResolutionDto(
    int ResolutionId,
    int CompanyProviderId,
     ResolutionType ResolutionType = default,
    string Description = "",
    string ResolutionNumber = "",
    string Prefix = "",
    int MultipleResolution = 0,
    int VigencyMonth = 0,
    int Automatic = 0,
    int InitialRange = 0,
    int FinalRange = 0,
    int CurrentRange = 0,
    DateTime ResolutionDate = default,
    DateTime CreationDate = default,
    DateTime ExpirationDate = default,
    int ExpirationDays = 0,
    int ExpirationNumber = 0,
    bool Active = false
);
