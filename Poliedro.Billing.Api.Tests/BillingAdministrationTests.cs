using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poliedro.Billing.Api.Administration;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

namespace Poliedro.Billing.Api.Tests;

public class BillingAdministrationTests
{
    [Fact]
    public async Task AuthenticatedProvisioningIsIdempotentAndKeepsKeysPrivate()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.WebHost.UseTestServer();
        builder.Services.Configure<BillingAdministrationOptions>(options =>
        {
            options.Enabled = true;
            options.ApiKey = new string('t', 40);
            options.LicenseUrl = "http://localhost/poliedro_billing/authorize/test?db=test";
        });
        builder.Services.AddDbContext<DataBaseContext>(options => options.UseSqlite(connection));
        builder.Services.AddHttpClient();
        await using var app = builder.Build();
        app.MapBillingAdministration();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            await db.Database.EnsureCreatedAsync();
            db.Server.Add(new ServerEntity { ServerId = 1, Ip = "localhost", DatabaseName = "isolated", DbUsername = "test", DbPassword = "unused", Port = 3306 });
            await db.SaveChangesAsync();
        }
        await app.StartAsync();
        var http = app.GetTestClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await http.GetAsync("/api/v1/admin/status")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await http.GetAsync("/api/v1/admin/clients/1/resolutions")).StatusCode);
        http.DefaultRequestHeaders.Add("X-Billing-Admin-Key", new string('t', 40));
        Assert.Equal(HttpStatusCode.OK, (await http.GetAsync("/api/v1/admin/status")).StatusCode);
        var input = new { companyId = 0, name = "Isolated test", nit = "TEST-ONLY", email = "" };
        var first = await http.PostAsJsonAsync("/api/v1/admin/clients", input);
        first.EnsureSuccessStatusCode();
        var id = (await first.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("companyId").GetInt32();
        var retry = await http.PostAsJsonAsync("/api/v1/admin/clients", input);
        Assert.Equal(id, (await retry.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("companyId").GetInt32());
        var conflict = await http.PostAsJsonAsync("/api/v1/admin/clients", new { companyId = id + 100, input.name, input.nit, input.email });
        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        var provider = new { companyProviderId = 0, companyId = id, serviceId = 1, environmentType = 0, apiKey = "synthetic-test-plemsi-key", active = false };
        var result = await http.PostAsJsonAsync("/api/v1/admin/providers", provider);
        result.EnsureSuccessStatusCode();
        var providerId = (await result.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("companyProviderId").GetInt32();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE dian_resolution_billing ADD COLUMN date_current TEXT NULL");
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO dian_resolution_billing
                (resolution_id, company_provider_id, document_type, description, resolution_number, prefix,
                 multiple_resolution, vigency_month, automatic, initial_range, final_range, current_number,
                 resolution_date, expiration_date, expiration_days, expiration_numbers, active)
                VALUES (1,{providerId},'FE','Test','123','TEST',0,12,0,1,999,30,'2026-01-01','2027-01-01',15,20,1)
                """);
        }
        Assert.Equal(HttpStatusCode.NotFound, (await http.GetAsync("/api/v1/admin/clients/99999/resolutions")).StatusCode);
        var resolutionResponse = await http.GetAsync($"/api/v1/admin/clients/{id}/resolutions");
        resolutionResponse.EnsureSuccessStatusCode();
        var resolutionJson = await resolutionResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var resolutions = resolutionJson.GetProperty("resolutions");
        Assert.Equal(1, resolutions.GetArrayLength());
        Assert.Equal(30, resolutions[0].GetProperty("currentNumber").GetInt32());
        Assert.Equal("not_configured", resolutions[0].GetProperty("lookupStatus").GetString());
        Assert.DoesNotContain("synthetic-test-plemsi-key", resolutionJson.ToString());
        var again = await http.PostAsJsonAsync("/api/v1/admin/providers", provider);
        Assert.Equal(providerId, (await again.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("companyProviderId").GetInt32());
        var catalog = await http.GetStringAsync("/api/v1/admin/providers");
        Assert.DoesNotContain("synthetic-test-plemsi-key", catalog);
        Assert.DoesNotContain("apiKey", catalog);
        (await http.PutAsJsonAsync($"/api/v1/admin/clients/{id}/state", new { active = true })).EnsureSuccessStatusCode();
        (await http.PutAsJsonAsync($"/api/v1/admin/clients/{id}/state", new { active = false })).EnsureSuccessStatusCode();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            Assert.Equal(1, await db.ClientBillingElectronic.CountAsync());
            Assert.Equal(1, await db.CompanyProvider.CountAsync());
            Assert.Equal(0, (await db.CompanyProvider.SingleAsync()).Active);
            Assert.False((await db.ClientBillingElectronic.SingleAsync()).Active);
        }
        await app.StopAsync();
    }
}
