namespace Poliedro.Billing.Application.NotifyResolution.Dtos;
    public record NotifyResolutionDto
    (
     DateTime ExpirationDate,
     string? ResolutionType,
     int? FinalRange,
     int? CurrentlyNumber,
     ExpirationAlertDto? ExpirationAlert
    );