using Microsoft.Extensions.Configuration;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CustomersId;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class CustomersIdRepositoryTests
{
    [Fact]
    public async Task Repository_requests_customer_with_bearer_token_and_deserializes_response_data()
    {
        HttpRequestMessage? captured = null;
        using var http = new HttpClient(new StubHandler((request, _) =>
        {
            captured = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"code":200,"success":true,"info":"ok","data":{"_id":"cust-1","fullName":"Test Client","email":"customer@example.invalid","isActive":true}}""")
            });
        }));
        var repository = new CustomersIdRepository(Configuration("http://billing.test"), http);

        var customer = await repository.GetByIdAsync("cust-1", "fake-token");

        Assert.Equal("cust-1", customer.Id);
        Assert.Equal("Test Client", customer.FullName);
        Assert.True(customer.IsActive);
        Assert.Equal("Bearer fake-token", captured!.Headers.Authorization!.ToString());
        Assert.Equal("http://billing.test/customer/cust-1", captured.RequestUri!.ToString());
        Assert.Contains(captured.Headers.Accept, header => header.MediaType == "application/json");
    }

    [Fact]
    public async Task Repository_returns_null_for_non_success_http_response()
    {
        using var http = new HttpClient(new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound))));
        var repository = new CustomersIdRepository(Configuration("http://billing.test"), http);

        Assert.Null(await repository.GetByIdAsync("missing", "fake-token"));
    }

    private static IConfiguration Configuration(string url) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?> { ["ApiPlemsi:BaseUrl"] = url }).Build();

    private sealed class StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => respond(request, cancellationToken);
    }
}
