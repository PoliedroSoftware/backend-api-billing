using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Application.Siigo.Contracts.Models;
using Poliedro.Billing.Application.TNS.Contracts.Models;
using Poliedro.Billing.Infraestructure.External.Siigo.ConfigModels;
using Poliedro.Billing.Infraestructure.External.Siigo.DomainServices;
using Poliedro.Billing.Infraestructure.External.TNS.DomainServices;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using SiigoSettingsService = Poliedro.Billing.Infraestructure.External.Siigo.Services.Imp.SettingsService;
using TnsSettingsService = Poliedro.Billing.Infraestructure.External.TNS.Services.Imp.SettingsService;

namespace Poliedro.Billing.Api.Tests;

public sealed class ExternalProviderIntegrationTests
{
    [Fact]
    public async Task SiigoAdapterPostsAuthorizedInvoiceAndParsesSuccess()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create()
                .WithPath("/invoices")
                .WithHeader("Authorization", "Bearer synthetic-siigo-token")
                .WithHeader("Partner-ID", "Test")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{}"));

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApiSiigo:hostUrl"] = provider.Urls[0],
            ["ApiSiigo:createInvoceAndSendDianEndPoint"] = "/invoices",
            ["ApiSiigo:mediaType"] = "application/json"
        }).Build();
        var settings = new SiigoSettingsService(config);
        Assert.Equal(provider.Urls[0], settings.GetSettings().HostUrl);
        Assert.Equal("application/json", settings.GetValue("mediaType"));
        Assert.Null(settings.GetValue("missing"));

        var service = new SiigoDomainService(settings);
        var response = await service.CreateAndSendDianAsync(
            [new Invoice()],
            "synthetic-siigo-token",
            CancellationToken.None);

        Assert.True(response.Value!.Status);
        Assert.Single(response.Value.Data);
        Assert.Single(provider.LogEntries);
    }

    [Fact]
    public async Task SiigoAdapterReturnsProviderErrorBodyForFailedHttpStatus()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/invoices").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(422).WithBody("{\"error\":\"invalid invoice\"}"));

        var settings = new SiigoSettingsService(SiigoConfiguration(provider.Urls[0]));
        var response = await new SiigoDomainService(settings).CreateAndSendDianAsync(
            [new Invoice()], "synthetic-token", CancellationToken.None);

        Assert.False(response.Value!.Status);
        Assert.Contains("invalid invoice", response.Value.Message);
        Assert.Single(provider.LogEntries);
    }

    [Fact]
    public async Task TnsAdapterSendsBranchQueryAndReturnsCreatedOrder()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create()
                .WithPath("/sales")
                .WithParam("codigosucursal", "BR-17")
                .WithHeader("Authorization", "Bearer synthetic-tns-token")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"status":true,"message":"Pedido creado","data":{"success":true,"response":"OK","pedidoID":"P-1","consecutivo":"C-1","asentado":true,"mensajeAsentado":"Aceptado","enlacePago":""}}"""));

        var config = TnsConfiguration(provider.Urls[0]);
        var settings = new TnsSettingsService(config);
        Assert.Equal(provider.Urls[0], settings.GetSettings().HostUrl);
        Assert.Equal("application/json", settings.GetValue("mediaType"));

        var response = await new TNSDomainService(settings).CreateAsync(
            [new Order()], "BR-17", "synthetic-tns-token", CancellationToken.None);

        Assert.True(response.Value!.Status);
        Assert.Equal("Pedido creado", response.Value.Message);
        Assert.Equal("P-1", Assert.Single(response.Value.Data).PedidoID);
        Assert.Single(provider.LogEntries);
    }

    [Fact]
    public async Task TnsAdapterPreservesProviderErrorAndNonSuccessfulStatus()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/sales").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(503).WithBody("servicio TNS no disponible"));

        var response = await new TNSDomainService(new TnsSettingsService(TnsConfiguration(provider.Urls[0])))
            .CreateAsync([new Order()], "BR-17", "synthetic-token", CancellationToken.None);

        Assert.False(response.Value!.Status);
        Assert.Equal("servicio TNS no disponible", response.Value.Message);
        Assert.Empty(response.Value.Data);
        Assert.Single(provider.LogEntries);
    }

    private static IConfiguration SiigoConfiguration(string hostUrl) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApiSiigo:hostUrl"] = hostUrl,
            ["ApiSiigo:createInvoceAndSendDianEndPoint"] = "/invoices",
            ["ApiSiigo:mediaType"] = "application/json"
        }).Build();

    private static IConfiguration TnsConfiguration(string hostUrl) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApiTNS:hostUrl"] = hostUrl,
            ["ApiTNS:createSaleEndPoint"] = "/sales",
            ["ApiTNS:mediaType"] = "application/json"
        }).Build();
}
