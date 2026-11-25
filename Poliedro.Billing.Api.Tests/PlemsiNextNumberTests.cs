using System.Net;
using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl.Plemsi;

namespace Poliedro.Billing.Api.Tests;

public class PlemsiNextNumberTests
{
    [Theory]
    [InlineData("{\"data\":{\"docs\":[]}}", 100, 100)]
    [InlineData("{\"data\":{\"docs\":[{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"103\"},{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"105\"}]}}", 100, 106)]
    [InlineData("{\"data\":{\"docs\":[{\"state\":\"Error\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"105\"},{\"state\":\"Emitted\",\"prefix\":\"OTHER\",\"resolution_number\":\"123\",\"number\":\"999\"},{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"OLD\",\"number\":\"888\"}]}}", 100, 100)]
    [InlineData("{\"data\":{\"docs\":[{\"state\":\"Deleted\",\"cude\":\"used-cude\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"104\"},{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"103\"}]}}", 100, 105)]
    public async Task UsesConsumedDocumentsInExactResolution(string response, int initial, int expected)
        => Assert.Equal(expected, await Query(response, initial));

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"data\":{\"docs\":null}}")]
    [InlineData("{\"data\":{\"docs\":{}}}")]
    [InlineData("{\"data\":{\"docs\":[{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"invalid\"}]}}")]
    [InlineData("{\"data\":{\"docs\":[{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"10\"},{\"state\":\"Emitted\",\"prefix\":\"QA\",\"resolution_number\":\"123\",\"number\":\"invalid\"}]}}")]
    [InlineData("{\"data\":{\"docs\":[{\"number\":\"10\"}]}}")]
    [InlineData("not json")]
    public async Task InvalidResponseNeverRestartsNumbering(string response)
        => await Assert.ThrowsAsync<InvalidOperationException>(() => Query(response, 1));

    [Fact]
    public async Task EmptyRangeRequiresPositiveInitialNumber()
        => await Assert.ThrowsAsync<InvalidOperationException>(() => Query("{\"data\":{\"docs\":[]}}", 0));

    private static Task<int> Query(string response, int initial)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Enviroment:Production"] = "true",
            ["ApiPlemsi:FELastInvoiceUrl"] = "https://example.invalid/documents?prefix=",
        }).Build();
        return new GetLastInvoiceBillingPlemsi(config, new Factory(response)).GetLastInvoiceNumberAsync(
            new DianResolutionEntity { InitialRange = initial, Prefix = "QA", ResolutionNumber = "123" },
            new CompanyProviderEntity { ApiKey = "synthetic-test-key" }, default);
    }

    private sealed class Factory(string response) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(new Handler(response));
    }
    private sealed class Handler(string response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(response) });
    }
}
