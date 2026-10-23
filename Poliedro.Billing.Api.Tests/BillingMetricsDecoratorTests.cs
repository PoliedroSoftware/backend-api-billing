using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Poliedro.Billing.Api.Observability.Decorators;
using Poliedro.Billing.Api.Observability.Providers;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.Responses;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingMetricsDecoratorTests
{
    [Fact]
    public async Task Fe_decorator_records_success_returns_response_and_disposes_scope()
    {
        var inner = new Mock<IBillingSender>();
        var metrics = new Mock<IProviderMetrics>();
        var scope = new Mock<IProviderMetricsScope>();
        metrics.SetupGet(x => x.ProviderName).Returns("test-provider");
        var response = new List<ApiResponseFERetailPos> { new() { Success = true } };
        metrics.Setup(x => x.BeginRequest()).Returns(scope.Object);
        inner.Setup(x => x.SendAsync(It.IsAny<PlemsiInvoiceRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await new BillingMetricsDecoratorFE(inner.Object, metrics.Object,
                NullLogger<BillingMetricsDecoratorFE>.Instance)
            .SendAsync(Request(), CancellationToken.None);

        Assert.Same(response, result);
        metrics.Verify(x => x.RecordSuccess(scope.Object, "create_invoice_fe"), Times.Once);
        metrics.Verify(x => x.RecordError(It.IsAny<IProviderMetricsScope>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [Theory]
    [InlineData("timeout", "create_invoice_fe")]
    [InlineData("http", "create_invoice_fe")]
    [InlineData("other", "create_invoice_fe")]
    public async Task Fe_decorator_records_errors_and_rethrows_original_exception(string failure, string requestType)
    {
        var inner = new Mock<IBillingSender>();
        var metrics = new Mock<IProviderMetrics>();
        var scope = new Mock<IProviderMetricsScope>();
        metrics.SetupGet(x => x.ProviderName).Returns("test-provider");
        metrics.Setup(x => x.BeginRequest()).Returns(scope.Object);
        Exception exception = failure switch
        {
            "timeout" => new TimeoutException("simulated"),
            "http" => new HttpRequestException("simulated", null, System.Net.HttpStatusCode.Unauthorized),
            _ => new InvalidOperationException("simulated")
        };
        inner.Setup(x => x.SendAsync(It.IsAny<PlemsiInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        var decorator = new BillingMetricsDecoratorFE(inner.Object, metrics.Object,
            NullLogger<BillingMetricsDecoratorFE>.Instance);

        var thrown = await Assert.ThrowsAsync(exception.GetType(), () => decorator.SendAsync(Request(), CancellationToken.None));

        Assert.Same(exception, thrown);
        var reason = failure switch
        {
            "timeout" => "timeout",
            "http" => "authentication_failed",
            _ => "unknown_error"
        };
        metrics.Verify(x => x.RecordError(scope.Object, requestType, reason), Times.Once);
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public async Task Pos_decorator_records_success_and_disposes_scope()
    {
        var inner = new Mock<IBillingSender>();
        var metrics = new Mock<IProviderMetrics>();
        var scope = new Mock<IProviderMetricsScope>();
        metrics.SetupGet(x => x.ProviderName).Returns("test-provider");
        var response = new List<ApiResponseFERetailPos> { new() { Success = true } };
        metrics.Setup(x => x.BeginRequest()).Returns(scope.Object);
        inner.Setup(x => x.SendAsync(It.IsAny<PlemsiInvoiceRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await new BillingMetricsDecoratorPOS(inner.Object, metrics.Object,
                NullLogger<BillingMetricsDecoratorPOS>.Instance)
            .SendAsync(Request(), CancellationToken.None);

        Assert.Same(response, result);
        metrics.Verify(x => x.RecordSuccess(scope.Object, "create_invoice_pos"), Times.Once);
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [Theory]
    [InlineData("timeout", "timeout")]
    [InlineData("unauthorized", "authentication_failed")]
    [InlineData("provider", "provider_error")]
    [InlineData("unknown", "unknown_error")]
    public async Task Pos_decorator_records_error_category_rethrows_and_disposes_scope(string failure, string expectedReason)
    {
        var inner = new Mock<IBillingSender>();
        var metrics = new Mock<IProviderMetrics>();
        var scope = new Mock<IProviderMetricsScope>();
        metrics.SetupGet(x => x.ProviderName).Returns("test-provider");
        metrics.Setup(x => x.BeginRequest()).Returns(scope.Object);
        Exception exception = failure switch
        {
            "timeout" => new TimeoutException("test timeout"),
            "unauthorized" => new HttpRequestException("auth rejected", null, System.Net.HttpStatusCode.Unauthorized),
            "provider" => new HttpRequestException("provider failed", null, System.Net.HttpStatusCode.ServiceUnavailable),
            _ => new InvalidOperationException("unexpected")
        };
        inner.Setup(x => x.SendAsync(It.IsAny<PlemsiInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var thrown = await Assert.ThrowsAsync(exception.GetType(), () =>
            new BillingMetricsDecoratorPOS(inner.Object, metrics.Object, NullLogger<BillingMetricsDecoratorPOS>.Instance)
                .SendAsync(Request(), CancellationToken.None));

        Assert.Same(exception, thrown);
        metrics.Verify(x => x.RecordError(scope.Object, "create_invoice_pos", expectedReason), Times.Once);
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    private static PlemsiInvoiceRequest Request() => new()
    {
        DianResolutionEntity = new DianResolutionEntity(),
        CompanyProviderEntity = new CompanyProviderEntity { ApiKey = "synthetic-test-key" },
        Invoices = []
    };
}
