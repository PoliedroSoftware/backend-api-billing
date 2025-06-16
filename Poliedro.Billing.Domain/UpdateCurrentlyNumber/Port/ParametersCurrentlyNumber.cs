namespace Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
public record ParametersCurrentlyNumber(
    int Invoice,
    string? CurrentlyDate,
    int ResolutionId,
    bool Expirated = false
);