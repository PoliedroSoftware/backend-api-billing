using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.PdfInvoice.DomainPdfInvoice;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class PdfInvoiceServiceTests
{
    [Fact]
    public async Task Pdf_service_downloads_base64_payload_with_bearer_token_and_decodes_bytes()
    {
        HttpRequestMessage? captured = null;
        using var http = new HttpClient(new StubHandler((request, _) =>
        {
            captured = request;
            var encoded = Convert.ToBase64String([1, 2, 3, 4]);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"{{\"data\":\"{encoded}\"}}") });
        }));
        var service = new PdfInvoiceService(Config("http://billing.test/pdf"), http);

        var result = await service.GetPdfBytes("doc-7", "fake-token");

        Assert.Equal(new byte[] { 1, 2, 3, 4 }, result);
        Assert.Equal("Bearer fake-token", captured!.Headers.Authorization!.ToString());
        Assert.Equal("http://billing.test/pdf/doc-7", captured.RequestUri!.ToString());
    }

    [Fact]
    public async Task Pdf_service_validates_identifier_and_rejects_empty_or_invalid_provider_payload()
    {
        using var neverCalledHttp = new HttpClient(new StubHandler((_, _) => throw new InvalidOperationException("not expected")));
        var service = new PdfInvoiceService(Config("http://billing.test/pdf"), neverCalledHttp);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetPdfBytes("", "fake-token"));

        using var emptyHttp = new HttpClient(new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") })));
        var emptyService = new PdfInvoiceService(Config("http://billing.test/pdf"), emptyHttp);
        var empty = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyService.GetPdfBytes("doc-1", "fake-token"));
        Assert.Contains("no contiene datos válidos", empty.Message);

        using var badBase64Http = new HttpClient(new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":\"not-base64\"}") })));
        await Assert.ThrowsAsync<InvalidOperationException>(() => new PdfInvoiceService(Config("http://billing.test/pdf"), badBase64Http).GetPdfBytes("doc-2", "fake-token"));
    }

    [Fact]
    public async Task Pdf_service_wraps_http_failure_and_requires_configuration()
    {
        using var http = new HttpClient(new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound))));
        var service = new PdfInvoiceService(Config("http://billing.test/pdf"), http);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPdfBytes("missing", "fake-token"));
        Assert.Contains("Error fetching PDF", exception.Message);

        Assert.Throws<ArgumentNullException>(() => new PdfInvoiceService(Config(null)));
    }

    private static IConfiguration Config(string? url) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?> { ["ApiPlemsi:PdfInvoiceUrl"] = url }).Build();

    private sealed class StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => respond(request, cancellationToken);
    }
}
