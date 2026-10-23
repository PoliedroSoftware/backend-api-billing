using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Logging;
using Poliedro.Billing.Application.Billing.Services.Factories;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;

namespace Poliedro.Billing.Api.Observability;

/// <summary>Applies provider-independent API telemetry to every registered billing adapter.</summary>
public sealed class BillingMetricsProviderAdapterFactory : IBillingProviderAdapterFactory
{
    private readonly IReadOnlyDictionary<int, IBillingProviderAdapter> _adapters;

    public BillingMetricsProviderAdapterFactory(
        IEnumerable<IBillingProviderAdapter> adapters,
        ILoggerFactory loggerFactory)
    {
        _adapters = adapters.ToDictionary(
            adapter => adapter.ProviderId,
            adapter => (IBillingProviderAdapter)new BillingMetricsProviderAdapter(
                adapter,
                loggerFactory.CreateLogger<BillingMetricsProviderAdapter>()));
    }

    public IBillingProviderAdapter Resolve(int providerId) =>
        _adapters.TryGetValue(providerId, out var adapter)
            ? adapter
            : throw new InvalidOperationException($"No billing adapter is registered for provider id '{providerId}'.");
}

internal sealed class BillingMetricsProviderAdapter(
    IBillingProviderAdapter inner,
    ILogger<BillingMetricsProviderAdapter> logger) : IBillingProviderAdapter
{
    public int ProviderId => inner.ProviderId;
    public string ProviderKey => inner.ProviderKey;

    public ProviderConfigurationValidation ValidateConfiguration(CompanyProviderEntity companyProvider) =>
        inner.ValidateConfiguration(companyProvider);

    public Task<int> GetLastNumberAsync(
        Poliedro.Billing.Domain.Resolution.Entities.DianResolutionEntity resolution,
        CompanyProviderEntity companyProvider,
        CancellationToken cancellationToken) => inner.GetLastNumberAsync(resolution, companyProvider, cancellationToken);

    public async Task<IReadOnlyList<BillingProviderResponse>> SendAsync(
        BillingProviderSubmission submission,
        CancellationToken cancellationToken)
    {
        var started = Stopwatch.GetTimestamp();
        var documentType = ToDocumentTypeLabel(submission.DocumentType);
        var clientId = submission.CompanyProvider.CompanyId.ToString();

        try
        {
            var responses = await inner.SendAsync(submission, cancellationToken);
            if (responses.Count == 0)
            {
                BillingMetrics.InvoiceFailedTotal
                    .WithLabels(ProviderKey, clientId, documentType, "empty_provider_response").Inc();
                logger.LogWarning("Provider {ProviderKey} returned no emission result for company {CompanyId}.",
                    ProviderKey, submission.CompanyProvider.CompanyId);
                return responses;
            }

            foreach (var response in responses)
            {
                if (response.Success)
                    BillingMetrics.InvoiceSuccessTotal.WithLabels(ProviderKey, clientId, documentType).Inc();
                else
                    BillingMetrics.InvoiceFailedTotal
                        .WithLabels(ProviderKey, clientId, documentType, "provider_rejected").Inc();
            }

            return responses;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            var reason = Classify(exception);
            BillingMetrics.InvoiceFailedTotal.WithLabels(ProviderKey, clientId, documentType, reason).Inc();
            logger.LogError(exception, "Provider {ProviderKey} failed to emit billing documents for company {CompanyId}.",
                ProviderKey, submission.CompanyProvider.CompanyId);
            throw;
        }
        finally
        {
            BillingMetrics.InvoiceDurationSeconds
                .WithLabels(ProviderKey, clientId, documentType)
                .Observe(Stopwatch.GetElapsedTime(started).TotalSeconds);
        }
    }

    private static string ToDocumentTypeLabel(ResolutionType documentType) => documentType switch
    {
        ResolutionType.FE => "FE",
        ResolutionType.POS => "POS",
        _ => documentType.ToString()
    };

    private static string Classify(Exception exception) => exception switch
    {
        TimeoutException => "timeout",
        TaskCanceledException => "timeout",
        HttpRequestException { StatusCode: HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden } => "authentication_failed",
        HttpRequestException { StatusCode: HttpStatusCode.BadRequest } => "invalid_request",
        HttpRequestException { StatusCode: >= HttpStatusCode.InternalServerError } => "provider_error",
        HttpRequestException => "http_error",
        _ => "unexpected_error"
    };
}
