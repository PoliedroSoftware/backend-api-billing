using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Api.Administration;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.CompanyProvider.Enums;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingAdministrationEndpointsIntegrationTests
{
    [Fact]
    public async Task Admin_endpoints_authenticate_then_create_client_provider_and_change_client_state()
    {
        await using var database = await TestBillingDatabase.CreateAsync();
        database.Context.ClientBillingElectronic.Add(new ClientEntity
        {
            CompanyId = 700,
            Name = "Cliente admin",
            Nit = "900700",
            Email = "admin@example.invalid",
            Active = true
        });
        database.Context.Server.Add(new ServerEntity
        {
            ServerId = 70,
            Ip = "sqlite-memory",
            DatabaseName = "isolated",
            DbUsername = "test",
            DbPassword = "test",
            Port = 3306
        });
        await database.Context.SaveChangesAsync();
        await using var app = await CreateApp(database);
        using var client = app.GetTestClient();

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/admin/status")).StatusCode);
        client.DefaultRequestHeaders.Add("X-Billing-Admin-Key", AdminKey);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/admin/status")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/admin/clients")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/admin/servers")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/admin/providers")).StatusCode);

        var invalidClient = await client.PostAsJsonAsync("/api/v1/admin/clients", new { CompanyId = 0, Name = "", Nit = "", Email = "" });
        Assert.Equal(HttpStatusCode.BadRequest, invalidClient.StatusCode);
        var upsertClient = await client.PostAsJsonAsync("/api/v1/admin/clients", new { CompanyId = 700, Name = "Cliente renombrado", Nit = "900700", Email = "nuevo@example.invalid" });
        Assert.Equal(HttpStatusCode.OK, upsertClient.StatusCode);

        var invalidProvider = await client.PostAsJsonAsync("/api/v1/admin/providers", new { CompanyProviderId = 0, CompanyId = 700, ServiceId = 70, EnvironmentType = 0, ApiKey = "", Active = true });
        Assert.Equal(HttpStatusCode.BadRequest, invalidProvider.StatusCode);
        var provider = await client.PostAsJsonAsync("/api/v1/admin/providers", new { CompanyProviderId = 0, CompanyId = 700, ServiceId = 70, EnvironmentType = 0, ApiKey = "synthetic-admin-test-key", Active = true });
        Assert.Equal(HttpStatusCode.OK, provider.StatusCode);

        var state = await client.PutAsJsonAsync("/api/v1/admin/clients/700/state", new { Active = false });
        Assert.Equal(HttpStatusCode.OK, state.StatusCode);
        database.Context.ChangeTracker.Clear();
        Assert.False((await database.Context.ClientBillingElectronic.SingleAsync(c => c.CompanyId == 700)).Active);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/v1/admin/clients/999/state", new { Active = true })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/admin/clients/700/emission-failures")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/v1/admin/clients/999/emission-failures")).StatusCode);
    }

    private static async Task<WebApplication> CreateApp(TestBillingDatabase database)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddDbContext<DataBaseContext>(options => options.UseSqlite(database.Context.Database.GetDbConnection()));
        builder.Services.Configure<BillingAdministrationOptions>(options =>
        {
            options.Enabled = true;
            options.ApiKey = AdminKey;
            options.LicenseUrl = "http://localhost/license";
        });
        var app = builder.Build();
        app.MapBillingAdministration();
        await app.StartAsync();
        return app;
    }

    private const string AdminKey = "synthetic-local-administration-key-000000000000";
}
