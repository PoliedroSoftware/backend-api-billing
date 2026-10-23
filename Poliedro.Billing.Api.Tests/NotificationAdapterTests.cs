using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Location;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendEmail;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class NotificationAdapterTests
{
    [Fact]
    public async Task Municipality_adapter_maps_provider_contract_and_sends_authorization_header()
    {
        var handler = new StubHttpHandler((request, _) =>
        {
            Assert.Equal("Bearer fake-key", request.Headers.Authorization!.ToString());
            Assert.Equal("https://api.plemsi.com/api/common/list/municipality/complete", request.RequestUri!.ToString());
            return Task.FromResult(JsonResponse("""{"success":true,"data":[{"id":1,"name":"Bogotá","code":"11001","department":{"id":11,"name":"Bogotá D.C.","code":"11"}}]}"""));
        });
        using var client = new HttpClient(handler);
        var result = await new PlemsiLocationService(client).GetAllAsync("fake-key", CancellationToken.None);

        var municipality = Assert.Single(result);
        Assert.Equal("Bogotá", municipality.Name);
        Assert.Equal("Bogotá D.C.", municipality.DepartmentName);
        Assert.Equal("11001", municipality.Divipola);
    }

    [Fact]
    public async Task Municipality_adapter_returns_empty_for_missing_data_and_throws_for_http_error()
    {
        using var emptyClient = new HttpClient(new StubHttpHandler((_, _) => Task.FromResult(JsonResponse("""{"success":true,"data":null}"""))));
        Assert.Empty(await new PlemsiLocationService(emptyClient).GetAllAsync("test", CancellationToken.None));

        using var errorClient = new HttpClient(new StubHttpHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))));
        await Assert.ThrowsAsync<HttpRequestException>(() => new PlemsiLocationService(errorClient).GetAllAsync("test", CancellationToken.None));
    }

    [Fact]
    public void Email_renderer_includes_greeting_message_additional_data_and_footer()
    {
        var html = new HtmlEmailBodyRenderer().Render(new EmailBodyDto(
            "Hola", "La factura falló", "Equipo Billing", new Dictionary<string, string> { ["Cliente"] = "Tienda de prueba" }));

        Assert.Contains("Hola<br><br>La factura falló", html);
        Assert.Contains("<strong>Cliente:</strong> Tienda de prueba<br>", html);
        Assert.EndsWith("<br><br>Equipo Billing", html);
    }

    [Fact]
    public async Task Smtp_sender_skips_smtp_when_username_is_not_configured()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var sender = new SmtpEmailSender(config, new HtmlEmailBodyRenderer(), NullLogger<SmtpEmailSender>.Instance);

        await sender.SendEmailAsync(new EmailMessageDto("recipient@example.invalid", "subject", new EmailBodyDto("Hi", "Body", "Bye", null)));
    }

    [Fact]
    public async Task WhatsApp_sender_posts_customer_invoice_and_configured_destination_without_exposing_token_in_url()
    {
        const string endpoint = "https://unit.invalid/messages";
        string? body = null;
        var handler = new StubHttpHandler(async (request, token) =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(endpoint, request.RequestUri!.ToString());
            Assert.Empty(request.RequestUri.Query);
            Assert.Equal("fake-test-token", request.Headers.GetValues("Token").Single());
            body = await request.Content!.ReadAsStringAsync(token);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        using var client = new HttpClient(handler);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["WhatsApp:Endpoint"] = endpoint,
            ["WhatsApp:Token"] = "fake-test-token",
            ["WhatsApp:Phone"] = "+570000000000"
        }).Build();

        await new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage.SendMessageService(client, configuration)
            .SendMessageAsync("FE-42", "Cliente prueba");

        using var json = System.Text.Json.JsonDocument.Parse(body!);
        Assert.Equal("+570000000000", json.RootElement.GetProperty("phone").GetString());
        Assert.Equal("Hola Cliente prueba, tu factura número FE-42 fue emitida a la Dian.", json.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task WhatsApp_sender_rejects_missing_configuration_and_propagates_provider_errors()
    {
        using var emptyClient = new HttpClient(new StubHttpHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        var missing = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage.SendMessageService(emptyClient, missing)
                .SendMessageAsync("FE-1", "Cliente"));

        using var failedClient = new HttpClient(new StubHttpHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))));
        var configured = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["WhatsApp:Endpoint"] = "http://localhost/messages",
            ["WhatsApp:Token"] = "fake-test-token",
            ["WhatsApp:Phone"] = "+570000000000"
        }).Build();
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.SendMessage.SendMessageService(failedClient, configured)
                .SendMessageAsync("FE-1", "Cliente"));
    }

    private static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
    };

    private sealed class StubHttpHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => handler(request, cancellationToken);
    }
}
