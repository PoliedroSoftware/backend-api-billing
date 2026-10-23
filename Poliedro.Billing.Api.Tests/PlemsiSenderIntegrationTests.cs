using Microsoft.Extensions.Configuration;
using Moq;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.FE;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.POS;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Contracts;
using Poliedro.Billing.Infraestructure.External.Plemsi.Contracts.Responses;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiSenderIntegrationTests
{
    private const string SuccessJson = "{\"code\":200,\"success\":true,\"info\":\"accepted\",\"data\":{}}";

    [Fact]
    public async Task Electronic_invoice_sender_uses_qa_endpoint_bearer_token_and_next_invoice_number()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/fe").WithHeader("Authorization", "Bearer fake-plemsi-token").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(SuccessJson));
        var sender = new BillingSenderFE(Config(provider, "/fe"), LastElectronicInvoice(8), ClientFactory(provider));

        var result = await sender.SendAsync(CreateRequest(
        [new SenderRequestFEDTO { date = "2026-01-01", time = "10:00:00", prefix = "TST", number = 3, customer = null!, payment = null!, items = [], resolution = "R-1" }]), CancellationToken.None);

        Assert.True(Assert.Single(result).Success);
        var body = Assert.Single(provider.LogEntries).RequestMessage.Body;
        Assert.Contains("\"number\":8", body);
    }

    [Fact]
    public async Task Pos_sender_returns_empty_for_provider_http_error()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/pos").UsingPost())
            .RespondWith(Response.Create().WithStatusCode((int)HttpStatusCode.BadRequest).WithBody("invalid"));
        var sender = new BillingSenderPOS(Config(provider, "/pos"), LastPosInvoice(4), ClientFactory(provider));

        var result = await sender.SendAsync(CreateRequest(
        [new InvoiceRequestPosDto { date = "2026-01-01", time = "10:00:00", sendToEmail = "test@example.invalid", resolution = "R-1", prefix = "TST", number = 1, invoiceBaseTotal = "0", invoiceTaxExclusiveTotal = "0" }]), CancellationToken.None);

        Assert.Empty(result);
        Assert.Single(provider.LogEntries);
        Assert.Contains("\"number\":4", provider.LogEntries.Single().RequestMessage.Body);
    }

    [Fact]
    public void Sender_factory_resolves_FE_and_POS_and_reports_unknown_resolution()
    {
        var fe = new BillingSenderFE(null!, null!, null!);
        var pos = new BillingSenderPOS(null!, null!, null!);
        var factory = new BillingSenderRegistry([fe, pos]);

        Assert.Same(fe, factory.Resolve(1, ResolutionType.FE));
        Assert.Same(pos, factory.Resolve(1, ResolutionType.POS));
        Assert.Throws<InvalidOperationException>(() => factory.Resolve(2, ResolutionType.FE));
        Assert.Throws<Exception>(() => new BillingSenderRegistry([Mock.Of<IBillingSender>()]));
    }

    private static IConfiguration Config(WireMockServer provider, string path) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Enviroment:Production"] = "false",
            ["ApiPlemsiQa:FEUrl"] = provider.Urls[0] + path,
            ["ApiPlemsiQa:PosUrl"] = provider.Urls[0] + path
        }).Build();

    private static IHttpClientFactory ClientFactory(WireMockServer provider)
    {
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient { BaseAddress = new Uri(provider.Urls[0]) });
        return factory.Object;
    }

    private static IGetLastInvoiceBilling LastElectronicInvoice(int number)
    {
        var service = new Mock<IGetLastInvoiceBilling>();
        service.Setup(x => x.GetLastInvoiceNumberAsync(It.IsAny<DianResolutionEntity>(), It.IsAny<CompanyProviderEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(number);
        return service.Object;
    }

    private static IInvoiceLastPos LastPosInvoice(int number)
    {
        var service = new Mock<IInvoiceLastPos>();
        service.Setup(x => x.GetInvoiceLastAsync(It.IsAny<DianResolutionEntity>(), It.IsAny<CompanyProviderEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(number);
        return service.Object;
    }

    private static PlemsiInvoiceRequest CreateRequest(IEnumerable<object> invoices) => new()
    {
        DianResolutionEntity = new DianResolutionEntity { ResolutionId = 1 },
        CompanyProviderEntity = new CompanyProviderEntity { CompanyProviderId = 2, CompanyId = 3, ApiKey = "fake-plemsi-token" },
        Invoices = invoices
    };
}
