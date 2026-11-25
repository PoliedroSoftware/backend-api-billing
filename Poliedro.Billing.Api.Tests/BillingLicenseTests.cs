using System.Net;
using Microsoft.Extensions.Options;
using Poliedro.Billing.Api.Administration;

namespace Poliedro.Billing.Api.Tests;

public class BillingLicenseTests
{
    private static BillingAdministrationOptions Settings() => new()
    {
        Enabled = true, ApiKey = new string('x', 40), LicenseUrl = "http://localhost:18079/poliedro_billing/authorize/test?db=odoo_test"
    };

    [Theory]
    [InlineData("{\"allowed\":true,\"code\":\"active\"}", true)]
    [InlineData("{\"allowed\":false,\"code\":\"license_suspended\"}", false)]
    [InlineData("{}", false)]
    [InlineData("invalid json", false)]
    [InlineData("null", false)]
    public async Task AuthorityMustExplicitlyApprove(string json, bool allowed)
    {
        var handler = new StubHandler(HttpStatusCode.OK, json);
        var service = new OdooBillingLicenseAuthorizer(new HttpClient(handler), Options.Create(Settings()));
        Assert.Equal(allowed, (await service.AuthorizeAsync(10, 20, default)).Allowed);
        Assert.Contains("\"companyProviderId\":20", handler.Body);
        Assert.Equal("Bearer", handler.Scheme);
        Assert.Equal("odoo_test", handler.Database);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.Found)]
    public async Task FailedAuthorityDenies(HttpStatusCode status)
    {
        var service = new OdooBillingLicenseAuthorizer(new HttpClient(new StubHandler(status, "{}")), Options.Create(Settings()));
        Assert.False((await service.AuthorizeAsync(1, 2, default)).Allowed);
    }

    [Fact]
    public async Task UnreachableAuthorityDenies()
    {
        var service = new OdooBillingLicenseAuthorizer(new HttpClient(new FailingHandler()), Options.Create(Settings()));
        Assert.False((await service.AuthorizeAsync(1, 2, default)).Allowed);
    }

    [Fact]
    public async Task EnabledButUnconfiguredDenies()
    {
        var options = Settings(); options.ApiKey = "";
        var service = new OdooBillingLicenseAuthorizer(new HttpClient(new FailingHandler()), Options.Create(options));
        Assert.False((await service.AuthorizeAsync(1, 2, default)).Allowed);
    }

    [Fact]
    public void AdminKeyRequiresStrongExactMatch()
    {
        Assert.True(BillingAdminFilter.Matches(new string('x', 40), new string('x', 40)));
        Assert.False(BillingAdminFilter.Matches("", new string('x', 40)));
        Assert.False(BillingAdminFilter.Matches("short", "short"));
        Assert.False(BillingAdminFilter.Matches(new string('y', 40), new string('x', 40)));
    }

    [Fact]
    public async Task CompanyAuthorityDoesNotChangeOtherClients()
    {
        var settings = Settings();
        settings.CompanyLicenseUrls[28] = "https://odoo.example/authorize/test?db=company28";
        var handler = new StubHandler(HttpStatusCode.OK, "{\"allowed\":true}");
        var service = new OdooBillingLicenseAuthorizer(new HttpClient(handler), Options.Create(settings));
        Assert.True((await service.AuthorizeAsync(28, 28, default)).Allowed);
        Assert.Equal("odoo.example", handler.Url!.Host);
        Assert.Equal("company28", handler.Database);
        await service.AuthorizeAsync(10, 20, default);
        Assert.Equal("localhost", handler.Url!.Host);
        Assert.Equal("odoo_test", handler.Database);
    }

    [Fact]
    public async Task InvalidCompanyAuthorityDeniesWithoutFallback()
    {
        var settings = Settings();
        settings.CompanyLicenseUrls[28] = "http://public.example/authorize";
        var service = new OdooBillingLicenseAuthorizer(new HttpClient(new FailingHandler()), Options.Create(settings));
        Assert.False((await service.AuthorizeAsync(28, 28, default)).Allowed);
    }

    private class StubHandler(HttpStatusCode status, string json) : HttpMessageHandler
    {
        public string? Body { get; private set; }
        public string? Scheme { get; private set; }
        public string? Database { get; private set; }
        public Uri? Url { get; private set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Body = await request.Content!.ReadAsStringAsync(ct);
            Url = request.RequestUri;
            Scheme = request.Headers.Authorization?.Scheme;
            Database = request.Headers.GetValues("X-Odoo-Database").Single();
            return new HttpResponseMessage(status) { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
        }
    }

    private class FailingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => throw new HttpRequestException("offline");
    }
}
