using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Poliedro.Billing.Api.Common.Configurations;

namespace Poliedro.Billing.Api.Tests;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task Validation_errors_are_returned_as_grouped_bad_request_problem_details()
    {
        var handler = CreateHandler("Production");
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/v1/billing";
        await using var body = new MemoryStream();
        context.Response.Body = body;
        var exception = new ValidationException([
            new ValidationFailure("InvoiceNumber", "required"),
            new ValidationFailure("InvoiceNumber", "format"),
            new ValidationFailure("Client", "missing")
        ]);

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        body.Position = 0;
        using var json = await JsonDocument.ParseAsync(body);
        var errors = json.RootElement.GetProperty("errors");
        Assert.Equal(2, errors.GetProperty("InvoiceNumber").GetArrayLength());
        Assert.Equal("missing", errors.GetProperty("Client")[0].GetString());
    }

    [Theory]
    [InlineData("Production", false)]
    [InlineData("Development", true)]
    public async Task Unhandled_errors_return_internal_server_error_and_only_show_details_in_development(
        string environmentName, bool includesDetail)
    {
        var handler = CreateHandler(environmentName);
        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;

        var handled = await handler.TryHandleAsync(context, new InvalidOperationException("secret diagnostic"), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        body.Position = 0;
        using var json = await JsonDocument.ParseAsync(body);
        Assert.Equal(includesDetail, json.RootElement.TryGetProperty("detail", out var detail)
            && detail.GetString() == "secret diagnostic");
    }

    private static GlobalExceptionHandler CreateHandler(string environmentName)
    {
        var environment = new TestHostEnvironment { EnvironmentName = environmentName };
        return new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance, environment);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "Billing.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
