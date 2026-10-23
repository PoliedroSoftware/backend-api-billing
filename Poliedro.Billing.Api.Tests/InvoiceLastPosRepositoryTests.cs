using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Poliedro.Billing.Api.Tests;

public sealed class InvoiceLastPosRepositoryTests
{
    [Fact]
    public async Task Uses_qa_multiple_resolution_endpoint_with_auth_and_returns_next_number()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/multiple/POS").WithHeader("Authorization", "Bearer synthetic-key").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200)
                .WithBody("""{"data":{"docs":[{"number":"5"},{"number":"24"},{"number":"not-a-number"}]}}"""));
        var configuration = Configuration(production: false, new Dictionary<string, string?>
        {
            ["ApiPlemsiQa:PosLastInvoiceUrl"] = provider.Urls[0] + "/single/",
            ["ApiPlemsiQa:PosLastInvoiceUrlMultipleResolution"] = provider.Urls[0] + "/multiple/"
        });
        var repository = new InvoiceLastPosRepository(configuration, new HttpClient());

        var nextNumber = await repository.GetInvoiceLastAsync(
            new DianResolutionEntity { Prefix = "POS", MultipleResolution = (int)MultipleResolution.Multiple },
            new CompanyProviderEntity { ApiKey = "synthetic-key" }, CancellationToken.None);

        Assert.Equal(25, nextNumber);
        Assert.Single(provider.LogEntries);
    }

    [Theory]
    [InlineData("EMPTY", """{"data":{"docs":[]}}""", 200)]
    [InlineData("INVALID", """{"data":{"docs":[{"number":"x"}]}}""", 200)]
    [InlineData("DOWN", "service unavailable", 503)]
    public async Task Rejects_missing_or_unusable_provider_sequences(string prefix, string body, int status)
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/single/" + prefix).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(status).WithBody(body));
        var configuration = Configuration(production: false, new Dictionary<string, string?>
        {
            ["ApiPlemsiQa:PosLastInvoiceUrl"] = provider.Urls[0] + "/single/"
        });
        var repository = new InvoiceLastPosRepository(configuration, new HttpClient());

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetInvoiceLastAsync(
            new DianResolutionEntity { Prefix = prefix }, new CompanyProviderEntity(), CancellationToken.None));
    }

    [Fact]
    public async Task Invalid_json_is_translated_to_a_clear_provider_error()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/single/BADJSON").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("not json"));
        var configuration = Configuration(production: false, new Dictionary<string, string?>
        {
            ["ApiPlemsiQa:PosLastInvoiceUrl"] = provider.Urls[0] + "/single/"
        });
        var repository = new InvoiceLastPosRepository(configuration, new HttpClient());

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetInvoiceLastAsync(
            new DianResolutionEntity { Prefix = "BADJSON" }, new CompanyProviderEntity(), CancellationToken.None));

        Assert.Contains("formato JSON", error.Message);
        Assert.IsType<Newtonsoft.Json.JsonReaderException>(error.InnerException);
    }

    [Fact]
    public async Task Missing_environment_endpoint_configuration_fails_before_any_http_request()
    {
        var configuration = Configuration(production: false, new Dictionary<string, string?>());
        var repository = new InvoiceLastPosRepository(configuration, new HttpClient());

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetInvoiceLastAsync(
            new DianResolutionEntity { Prefix = "POS" }, new CompanyProviderEntity(), CancellationToken.None));

        Assert.Contains("URL de consulta", error.Message);
    }

    [Fact]
    public async Task Authorization_is_per_request_and_does_not_leak_between_companies()
    {
        using var provider = WireMockServer.Start();
        provider.Given(Request.Create().WithPath("/single/A").WithHeader("Authorization", "Bearer key-A").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{"data":{"docs":[{"number":"1"}]}}"""));
        provider.Given(Request.Create().WithPath("/single/B").WithHeader("Authorization", "Bearer key-B").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("""{"data":{"docs":[{"number":"2"}]}}"""));
        var configuration = Configuration(production: false, new Dictionary<string, string?>
        {
            ["ApiPlemsiQa:PosLastInvoiceUrl"] = provider.Urls[0] + "/single/"
        });
        var repository = new InvoiceLastPosRepository(configuration, new HttpClient());

        Assert.Equal(2, await repository.GetInvoiceLastAsync(new DianResolutionEntity { Prefix = "A" }, new CompanyProviderEntity { ApiKey = "key-A" }, CancellationToken.None));
        Assert.Equal(3, await repository.GetInvoiceLastAsync(new DianResolutionEntity { Prefix = "B" }, new CompanyProviderEntity { ApiKey = "key-B" }, CancellationToken.None));
        Assert.Equal(2, provider.LogEntries.Count);
    }

    private static IConfiguration Configuration(bool production, Dictionary<string, string?> settings)
    {
        settings["Enviroment:Production"] = production.ToString();
        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }
}
