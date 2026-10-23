using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicePos;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class InvoicePosRepositoryIntegrationTests
{
    [Fact]
    public async Task Repository_fetches_and_maps_invoice_documents_using_configured_credentials()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHandler((request, _) =>
        {
            captured = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"data":{"docs":[{"_id":"doc-1","company":"Test Co","state":"accepted","number":"17","prefix":"TST","response":{"message":"accepted","data":{"IsValid":"true","StatusDescription":"OK","IssueDate":"2026-01-01","IssueTime":"10:00:00","Filename":"invoice.xml","ErrorMessage":{"string":""}}}}]}}""")
            });
        });
        using var http = new HttpClient(handler);
        var repository = new InvoicePosRepository(Configuration("http://billing.test/invoices", "fake-api-key"), http);

        var invoice = Assert.Single(await repository.GetAllAsync(null, CancellationToken.None));

        Assert.Equal("doc-1", invoice._id);
        Assert.Equal("17", invoice.number);
        Assert.Equal("accepted", invoice.response.message);
        Assert.Equal("true", invoice.response.data!.IsValid);
        Assert.Equal("Bearer fake-api-key", captured!.Headers.Authorization!.ToString());
        Assert.Equal("?page=1&perPage=100", captured.RequestUri!.Query);
    }

    [Fact]
    public async Task Repository_returns_empty_when_provider_has_no_documents()
    {
        using var http = new HttpClient(new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"data":{"docs":[]}}""")
        })));
        var repository = new InvoicePosRepository(Configuration("http://billing.test/invoices", "fake-api-key"), http);

        Assert.Empty(await repository.GetAllAsync("TST", CancellationToken.None));
    }

    [Fact]
    public async Task Repository_surfaces_configuration_and_provider_failures_with_context()
    {
        var missingUrl = new InvoicePosRepository(Configuration(null, "fake-api-key"), new HttpClient(new StubHandler((_, _) => throw new InvalidOperationException("request should not be sent"))));
        var configError = await Assert.ThrowsAsync<Exception>(() => missingUrl.GetAllAsync(null, CancellationToken.None));
        Assert.Contains("URL configuration is missing", configError.Message);

        using var http = new HttpClient(new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("provider unavailable")
        })));
        var repository = new InvoicePosRepository(Configuration("http://billing.test/invoices", "fake-api-key"), http);
        var providerError = await Assert.ThrowsAsync<Exception>(() => repository.GetAllAsync(null, CancellationToken.None));
        Assert.Contains("Error in GetAllAsync", providerError.Message);
        Assert.IsType<HttpRequestException>(providerError.InnerException);
    }

    private static IConfiguration Configuration(string? url, string? key) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApiPlemsi:PosUrl"] = url,
            ["ApiPlemsi:ApiKey"] = key
        }).Build();

    private sealed class StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => respond(request, cancellationToken);
    }
}
