using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Poliedro.Billing.Api.Endpoints.v1.Billing;
using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Dtos;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingEndpointIntegrationTests
{
    [Fact]
    public async Task HangfireStyleRequest_ReachesFullApiAndDispatchesResolutionAndInvoices()
    {
        var mediator = CreateMediator();
        var proxy = (CapturingMediatorProxy)(object)mediator;
        proxy.Result = CreateBillingCommandResult.Ok(
        [
            new CreateBillingResultDTO { Status = true, Message = "Factura procesada" }
        ]);

        using var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(WebHostDefaults.EnvironmentKey, "Testing");
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IMediator>();
                    services.AddSingleton(mediator);
                });
            });
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/billing/57",
            new { data = new[] { new { number = "TST-9001" } } });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(proxy.Command);
        Assert.Equal(57, proxy.Command.Id);
        var invoice = Assert.Single(proxy.Command.Invoices);
        Assert.Equal("TST-9001", invoice.Number);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(body.RootElement[0].GetProperty("status").GetBoolean());
        Assert.Equal("Factura procesada", body.RootElement[0].GetProperty("message").GetString());
    }

    [Fact]
    public async Task PostBilling_DispatchesCommandAndReturnsSuccessfulResult()
    {
        var mediator = CreateMediator();
        var proxy = (CapturingMediatorProxy)(object)mediator;
        proxy.Result = CreateBillingCommandResult.Ok(
        [
            new CreateBillingResultDTO { Status = true, Message = "Factura procesada" }
        ]);

        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        var response = await client.PostAsJsonAsync("/api/v1/billing/57", new { data = Array.Empty<object>() });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(proxy.Command);
        Assert.Equal(57, proxy.Command.Id);
        Assert.Empty(proxy.Command.Invoices);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(body.RootElement[0].GetProperty("status").GetBoolean());
        Assert.Equal("Factura procesada", body.RootElement[0].GetProperty("message").GetString());
    }

    [Fact]
    public async Task PostBilling_WhenUseCaseReturnsNotFound_MapsStatusAndErrorBody()
    {
        var mediator = CreateMediator();
        var proxy = (CapturingMediatorProxy)(object)mediator;
        proxy.Result = CreateBillingCommandResult.NotFound("Resolución inexistente");

        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();

        var response = await client.PostAsJsonAsync("/api/v1/billing/999", new { data = Array.Empty<object>() });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(404, body.RootElement.GetProperty("statusCode").GetInt32());
        Assert.False(body.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal("Resolución inexistente", body.RootElement.GetProperty("message").GetString());
        Assert.Equal(999, proxy.Command?.Id);
    }

    [Fact]
    public async Task PostBilling_WithMalformedJson_ReturnsBadRequestWithoutDispatching()
    {
        var mediator = CreateMediator();
        var proxy = (CapturingMediatorProxy)(object)mediator;
        await using var app = await CreateApp(mediator);
        using var client = app.GetTestClient();
        using var content = new StringContent("{ invalid json", System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/v1/billing/57", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Null(proxy.Command);
    }

    private static IMediator CreateMediator() => DispatchProxy.Create<IMediator, CapturingMediatorProxy>();

    private static async Task<WebApplication> CreateApp(IMediator mediator)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton(mediator);

        var app = builder.Build();
        app.MapGroup("/api/v1/billing").MapBillingEndpoints();
        await app.StartAsync();
        return app;
    }
}

public class CapturingMediatorProxy : DispatchProxy
{
    public CreateBillingCommand? Command { get; private set; }

    public CreateBillingCommandResult Result { get; set; } =
        CreateBillingCommandResult.Ok(Array.Empty<CreateBillingResultDTO>());

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod?.Name == "Send" && args?.FirstOrDefault() is CreateBillingCommand command)
        {
            Command = command;
            return Task.FromResult(Result);
        }

        throw new NotSupportedException($"Unexpected mediator call: {targetMethod?.Name}");
    }
}
