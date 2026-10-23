using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Poliedro.Billing.Api.Tests;

public sealed class ApiStartupIntegrationTests
{
    [Fact]
    public async Task Api_starts_without_external_connections_and_publishes_configured_routes_and_security_contract()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting(WebHostDefaults.EnvironmentKey, "Testing"));

        using var client = factory.CreateClient();
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.Equal("Poliedro Billing API", root.GetProperty("info").GetProperty("title").GetString());
        Assert.True(root.GetProperty("components").GetProperty("securitySchemes").TryGetProperty("Bearer", out _));

        var paths = root.GetProperty("paths").EnumerateObject().Select(path => path.Name).ToArray();
        Assert.Contains("/api/v1/billing/{id}", paths);
        Assert.Contains(paths, path => path.Contains("companyprovider", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(paths, path => path.Contains("dianresolution", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(paths, path => path.Contains("invoicespendingwithdetails", StringComparison.OrdinalIgnoreCase));
    }
}
