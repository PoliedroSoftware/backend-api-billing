namespace Poliedro.Billing.Domain.Billing;

/// <summary>Normalized outcome returned by any electronic invoicing provider.</summary>
public sealed record BillingProviderResponse(
    int Code,
    bool Success,
    string? Message,
    string? Cude,
    string? QrCode,
    string? FootNote);
