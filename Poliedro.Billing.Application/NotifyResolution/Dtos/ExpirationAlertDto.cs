namespace Poliedro.Billing.Application.NotifyResolution.Dtos;
    public record ExpirationAlertDto
    (
        bool IsExpired,
        bool IsCloseToExpire,
        string? Message,
        int? DaysToExpire,
        int? RemainingNumeration,
        bool StatusNumeration);