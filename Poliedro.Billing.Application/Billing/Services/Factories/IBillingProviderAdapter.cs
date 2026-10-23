using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Application.Billing.Services.Factories;

/// <summary>Application boundary for sending prepared billing documents to a provider.</summary>
public interface IBillingProviderAdapter
{
    int ProviderId { get; }
    string ProviderKey { get; }

    ProviderConfigurationValidation ValidateConfiguration(CompanyProviderEntity companyProvider);

    Task<int> GetLastNumberAsync(
        DianResolutionEntity resolution,
        CompanyProviderEntity companyProvider,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<BillingProviderResponse>> SendAsync(
        BillingProviderSubmission submission,
        CancellationToken cancellationToken);
}

public sealed record ProviderConfigurationValidation(bool IsValid, string? Error)
{
    public static ProviderConfigurationValidation Valid() => new(true, null);
    public static ProviderConfigurationValidation Invalid(string error) => new(false, error);
}

public sealed record BillingProviderSubmission(
    DianResolutionEntity Resolution,
    CompanyProviderEntity CompanyProvider,
    ResolutionType DocumentType,
    IReadOnlyList<PreparedBillingDocument> PreparedDocuments);
