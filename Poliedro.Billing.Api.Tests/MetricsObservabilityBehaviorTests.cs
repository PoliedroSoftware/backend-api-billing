using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.TestHost;
using Poliedro.Billing.Api.Observability;
using Poliedro.Billing.Api.Observability.Pipeline;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Services.Factories;

namespace Poliedro.Billing.Api.Tests;

public sealed class MetricsObservabilityBehaviorTests
{
    [Fact]
    public async Task Non_billing_request_passes_through_without_replacing_response()
    {
        var behavior = new MetricsObservabilityBehavior<Ping, int>(NullLogger<MetricsObservabilityBehavior<Ping, int>>.Instance);
        var result = await behavior.Handle(new Ping(), _ => Task.FromResult(42), CancellationToken.None);
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task Billing_request_passes_successful_response_through()
    {
        var behavior = new MetricsObservabilityBehavior<CreateBillingCommand, CreateBillingCommandResult>(
            NullLogger<MetricsObservabilityBehavior<CreateBillingCommand, CreateBillingCommandResult>>.Instance);
        var expected = CreateBillingCommandResult.Ok([]);
        var result = await behavior.Handle(new CreateBillingCommand(123456789, []), _ => Task.FromResult(expected), CancellationToken.None);
        Assert.Same(expected, result);
    }

    [Fact]
    public async Task Billing_request_logs_then_rethrows_handler_exception()
    {
        var behavior = new MetricsObservabilityBehavior<CreateBillingCommand, CreateBillingCommandResult>(
            NullLogger<MetricsObservabilityBehavior<CreateBillingCommand, CreateBillingCommandResult>>.Instance);
        var expected = new TimeoutException("synthetic timeout");
        var thrown = await Assert.ThrowsAsync<TimeoutException>(() => behavior.Handle(
            new CreateBillingCommand(1, []), _ => Task.FromException<CreateBillingCommandResult>(expected), CancellationToken.None));
        Assert.Same(expected, thrown);
    }

    [Fact]
    public void Observability_registration_adds_provider_adapter_metrics_factory_and_mediatr_behavior()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddObservability();
        using var provider = services.BuildServiceProvider();

        Assert.IsType<Poliedro.Billing.Api.Observability.BillingMetricsProviderAdapterFactory>(
            provider.GetRequiredService<IBillingProviderAdapterFactory>());
        Assert.Contains(provider.GetServices<IPipelineBehavior<Ping, int>>(), behavior => behavior is MetricsObservabilityBehavior<Ping, int>);
    }

    [Fact]
    public async Task Prometheus_extensions_configure_middleware_and_metrics_endpoint()
    {
        var builder = WebApplication.CreateBuilder();
        await using var app = builder.Build();

        Assert.Same(app, app.UsePrometheusMetrics());
        Assert.Same(app, app.MapPrometheusMetrics());
    }

    [Fact]
    public async Task Provider_metrics_record_success_error_and_health_in_the_exposed_metrics()
    {
        var metrics = new Poliedro.Billing.Api.Observability.Providers.PlemsiMetrics();
        var successScope = metrics.BeginRequest();
        metrics.RecordSuccess(successScope, "coverage_contract_success");
        successScope.Dispose();

        var errorScope = metrics.BeginRequest();
        metrics.RecordError(errorScope, "coverage_contract_error", "synthetic_failure");
        errorScope.Dispose();
        metrics.SetProviderStatus(1);

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        await using var app = builder.Build();
        app.UsePrometheusMetrics().MapPrometheusMetrics();
        await app.StartAsync();
        using var client = app.GetTestClient();

        var response = await client.GetAsync("/metrics");
        response.EnsureSuccessStatusCode();
        var exposition = await response.Content.ReadAsStringAsync();

        Assert.Contains("billing_plemsi_requests_total{request_type=\"coverage_contract_success\"} 1", exposition);
        Assert.Contains("billing_plemsi_errors_total{request_type=\"coverage_contract_error\",error_reason=\"synthetic_failure\"} 1", exposition);
        Assert.Contains("billing_provider_status{provider=\"plemsi\"} 1", exposition);
        Assert.Contains("billing_plemsi_duration_seconds_count{request_type=\"coverage_contract_success\"} 1", exposition);
    }

    [Fact]
    public void Provider_metrics_ignore_scopes_created_by_another_provider()
    {
        var metrics = new Poliedro.Billing.Api.Observability.Providers.PlemsiMetrics();
        using var foreignScope = new FakeScope();

        metrics.RecordSuccess(foreignScope, "not_recorded");
        metrics.RecordError(foreignScope, "not_recorded", "not_recorded");
        metrics.SetProviderStatus(0);

        Assert.Equal("plemsi", metrics.ProviderName);
    }

    private sealed class FakeScope : Poliedro.Billing.Api.Observability.Providers.IProviderMetricsScope
    {
        public DateTime StartTime => DateTime.UtcNow;
        public void Dispose() { }
    }

    private sealed record Ping : IRequest<int>;
}
