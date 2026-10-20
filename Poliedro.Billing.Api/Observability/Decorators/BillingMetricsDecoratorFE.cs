using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Api.Observability.Providers;

namespace Poliedro.Billing.Api.Observability.Decorators;


public class BillingMetricsDecoratorFE : IBillingSender
{
    private readonly IBillingSender _innerSender;
    private readonly IProviderMetrics _metrics;
    private readonly ILogger<BillingMetricsDecoratorFE> _logger;

    public BillingMetricsDecoratorFE(
        IBillingSender innerSender,
        IProviderMetrics metrics,
        ILogger<BillingMetricsDecoratorFE> logger)
    {
        _innerSender = innerSender;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task<List<ApiResponseFERetailPos>> SendAsync(
        PlemsiInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var scope = _metrics.BeginRequest();
        const string requestType = "create_invoice_fe";

        try
        {
            _logger.LogInformation("Iniciando llamada a Plemsi (FE) - Request Type: {RequestType}", requestType);

            var result = await _innerSender.SendAsync(request, cancellationToken);

            // Éxito: registrar en métricas del proveedor
            _metrics.RecordSuccess(scope, requestType);

            // Registrar métrica de negocio (billing) con labels del proveedor
            var clientId = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceSuccessTotal
                .WithLabels(_metrics.ProviderName, clientId, "FE")
                .Inc();

            _logger.LogInformation("Llamada a Plemsi (FE) exitosa");

            return result;
        }
        catch (TimeoutException ex)
        {
            // Categoría: timeout
            _metrics.RecordError(scope, requestType, "timeout");
            var clientIdTimeout = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceFailedTotal
                .WithLabels(_metrics.ProviderName, clientIdTimeout, "FE", "timeout")
                .Inc();
            _logger.LogError(ex, "Timeout en llamada a Plemsi (FE)");
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Categorizar por HTTP status
            var errorReason = ex.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "authentication_failed",
                System.Net.HttpStatusCode.BadRequest => "invalid_request",
                >= System.Net.HttpStatusCode.InternalServerError => "provider_error",
                _ => "http_error"
            };
            _metrics.RecordError(scope, requestType, errorReason);
            var clientIdHttp = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceFailedTotal
                .WithLabels(_metrics.ProviderName, clientIdHttp, "FE", errorReason)
                .Inc();
            _logger.LogError(ex, "HTTP error en Plemsi (FE): {ErrorReason}", errorReason);
            throw;
        }
        catch (Exception ex)
        {
            // Otras excepciones
            _metrics.RecordError(scope, requestType, "unknown_error");
            var clientIdUnknown = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceFailedTotal
                .WithLabels(_metrics.ProviderName, clientIdUnknown, "FE", "unknown_error")
                .Inc();
            _logger.LogError(ex, "Error inesperado en Plemsi (FE)");
            throw;
        }
        finally
        {
            scope?.Dispose();
        }
    }
}


public class BillingMetricsDecoratorPOS : IBillingSender
{
    private readonly IBillingSender _innerSender;
    private readonly IProviderMetrics _metrics;
    private readonly ILogger<BillingMetricsDecoratorPOS> _logger;

    public BillingMetricsDecoratorPOS(
        IBillingSender innerSender,
        IProviderMetrics metrics,
        ILogger<BillingMetricsDecoratorPOS> logger)
    {
        _innerSender = innerSender;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task<List<ApiResponseFERetailPos>> SendAsync(
        PlemsiInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var scope = _metrics.BeginRequest();
        const string requestType = "create_invoice_pos";

        try
        {
            _logger.LogInformation("Iniciando llamada a Plemsi (POS) - Request Type: {RequestType}", requestType);

            var result = await _innerSender.SendAsync(request, cancellationToken);

            _metrics.RecordSuccess(scope, requestType);

            var clientId = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceSuccessTotal
                .WithLabels(_metrics.ProviderName, clientId, "POS")
                .Inc();

            _logger.LogInformation("Llamada a Plemsi (POS) exitosa");

            return result;
        }
        catch (TimeoutException ex)
        {
            _metrics.RecordError(scope, requestType, "timeout");
            var clientIdTimeoutPos = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceFailedTotal
                .WithLabels(_metrics.ProviderName, clientIdTimeoutPos, "POS", "timeout")
                .Inc();
            _logger.LogError(ex, "Timeout en llamada a Plemsi (POS)");
            throw;
        }
        catch (HttpRequestException ex)
        {
            var errorReason = ex.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "authentication_failed",
                System.Net.HttpStatusCode.BadRequest => "invalid_request",
                >= System.Net.HttpStatusCode.InternalServerError => "provider_error",
                _ => "http_error"
            };
            _metrics.RecordError(scope, requestType, errorReason);
            var clientIdHttpPos = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceFailedTotal
                .WithLabels(_metrics.ProviderName, clientIdHttpPos, "POS", errorReason)
                .Inc();
            _logger.LogError(ex, "HTTP error en Plemsi (POS): {ErrorReason}", errorReason);
            throw;
        }
        catch (Exception ex)
        {
            _metrics.RecordError(scope, requestType, "unknown_error");
            var clientIdUnknownPos = request.CompanyProviderEntity.ApiKey?.Substring(0, Math.Min(8, request.CompanyProviderEntity.ApiKey?.Length ?? 0)) ?? "unknown";
            Poliedro.Billing.Api.Observability.BillingMetrics.InvoiceFailedTotal
                .WithLabels(_metrics.ProviderName, clientIdUnknownPos, "POS", "unknown_error")
                .Inc();
            _logger.LogError(ex, "Error inesperado en Plemsi (POS)");
            throw;
        }
        finally
        {
            scope?.Dispose();
        }
    }
}
