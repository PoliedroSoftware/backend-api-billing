using Poliedro.Billing.Domain.Resolution.Enums;
namespace Poliedro.Billing.Application.DianResolution.Dtos;

public record DianResolutionCreditNoteDTO
(
    int ResolutionId,
    string ResolutionNumber = "",
    string Prefix = "",
    int InitialRange = 0,
    int FinalRange = 0,
    DateTime ResolutionDate = default,
    string Description = "",
    string ResolutionFile = "",
    bool Active = false,
    DateTime CreationDate = default,
    int VigencyMonth = 0,
    DateTime ExpirationDate = default,
    ResolutionType ResolutionType = default,
    int ClientBillingElectronicId = default,
    DateTime ExpirationDay = default,
    int ExpirationNumber = 0,
    int CurrentlyNumber = 0,
     DateTime CurrentlyDate = default
);
