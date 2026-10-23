using Microsoft.Extensions.Configuration;
using Moq;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiSequenceServiceTests
{
    [Fact]
    public async Task Sequence_service_uses_resolution_mode_and_returns_next_numeric_consecutive()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/multiple/TST").WithHeader("Authorization", "Bearer fake-token").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{"data":{"docs":[{"number":"4"},{"number":"19"},{"number":"invalid"}]}}"""));
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Enviroment:Production"] = "false",
            ["ApiPlemsiQa:FELastInvoiceUrl"] = provider.Urls[0] + "/single/",
            ["ApiPlemsiQa:FELastInvoiceUrlMultipleResolution"] = provider.Urls[0] + "/multiple/"
        }).Build();
        var httpFactory = new Mock<IHttpClientFactory>();
        httpFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());
        var service = new GetLastInvoiceBillingPlemsi(config, httpFactory.Object);

        var result = await service.GetLastInvoiceNumberAsync(
            new DianResolutionEntity { Prefix = "TST", MultipleResolution = (int)MultipleResolution.Multiple },
            new CompanyProviderEntity { ApiKey = "fake-token" }, CancellationToken.None);

        Assert.Equal(20, result);
        Assert.Single(provider.LogEntries);
    }

    [Fact]
    public async Task Sequence_service_rejects_http_failure_empty_documents_and_unusable_consecutives()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/single/EMPTY").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{"data":{"docs":[]}}"""));
        provider.Given(Request.Create().WithPath("/single/BAD").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{"data":{"docs":[{"number":"x"}]}}"""));
        provider.Given(Request.Create().WithPath("/single/DOWN").UsingGet())
            .RespondWith(Response.Create().WithStatusCode((int)HttpStatusCode.ServiceUnavailable));
        provider.Given(Request.Create().WithPath("/single/MALFORMED").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("not json"));
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Enviroment:Production"] = "false",
            ["ApiPlemsiQa:FELastInvoiceUrl"] = provider.Urls[0] + "/single/"
        }).Build();
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());
        var service = new GetLastInvoiceBillingPlemsi(config, factory.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLastInvoiceNumberAsync(new DianResolutionEntity { Prefix = "EMPTY" }, new(), CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLastInvoiceNumberAsync(new DianResolutionEntity { Prefix = "BAD" }, new(), CancellationToken.None));
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLastInvoiceNumberAsync(new DianResolutionEntity { Prefix = "DOWN" }, new(), CancellationToken.None));
        var malformed = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetLastInvoiceNumberAsync(new DianResolutionEntity { Prefix = "MALFORMED" }, new(), CancellationToken.None));
        Assert.Contains("formato JSON", malformed.Message);
    }
}
