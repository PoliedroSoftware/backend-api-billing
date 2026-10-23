using Microsoft.Extensions.Options;
using Poliedro.Billing.Api.Administration;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Poliedro.Billing.Api.Tests;

public sealed class OdooLicenseWireMockIntegrationTests
{
    [Fact]
    public async Task AuthorizerAcceptsAnExplicitApprovalFromOdooOverHttp()
    {
        using var odoo = WireMockServer.Start();
        odoo.Given(Request.Create()
                .WithPath("/poliedro_billing/authorize/test")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("""{"allowed":true,"code":"active"}"""));

        var settings = new BillingAdministrationOptions
        {
            Enabled = true,
            ApiKey = new string('x', 40),
            LicenseUrl = $"{odoo.Urls[0]}/poliedro_billing/authorize/test?db=odoo_test"
        };
        using var http = new HttpClient();
        var authorizer = new OdooBillingLicenseAuthorizer(http, Options.Create(settings));

        var decision = await authorizer.AuthorizeAsync(10, 20, CancellationToken.None);

        Assert.True(decision.Allowed);
        Assert.Equal("active", decision.Code);
        Assert.Single(odoo.LogEntries);
    }
}
