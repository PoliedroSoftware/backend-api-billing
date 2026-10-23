using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Poliedro.Billing.Application.Common.Behaviors;
using Poliedro.Billing.Application.Helper.DigitalVerification;
using Poliedro.Billing.Application.Helper.EmailBuilder;
using Poliedro.Billing.Application.SendEmail.Dtos;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Resolution.Entities;

namespace Poliedro.Billing.Api.Tests;

public sealed class ApplicationUtilityTests
{
    [Theory]
    [InlineData("900373913", "4")]
    [InlineData("abc", "error")]
    [InlineData("", "error")]
    public void Digital_verification_returns_check_digit_or_invalid_input_marker(string id, string expected)
        => Assert.Equal(expected, DigitalVerification.CalcularDigitoVerificacion(id));

    [Fact]
    public void Expired_resolution_message_explains_range_or_date_and_includes_contact_recipients()
    {
        var client = new ClientEntity { Email = "client@example.invalid", Name = "Test Client" };
        var resolution = new DianResolutionEntity { FinalRange = 5, InitialRange = 1, ExpirationDate = new DateTime(2026, 10, 1), ResolutionNumber = "R-5", Description = "FE", CurrentRange = 5 };

        var byRange = EmailMessageBuilder.BuildResolutionExpiredMessage(client, resolution, 6, "billing@example.invalid");
        Assert.Equal("client@example.invalid,billing@example.invalid", byRange.To);
        Assert.Contains("por numeración", byRange.Body!.Message);
        Assert.Equal("Test Client", byRange.Body.AdditionalData!["Cliente"]);

        var byDate = EmailMessageBuilder.BuildResolutionExpiredMessage(client, resolution, 5, "billing@example.invalid", "Aviso");
        Assert.Contains("por fecha", byDate.Body!.Message);
        Assert.Equal("Aviso", byDate.Subject);
    }

    [Fact]
    public void Failed_billing_message_deduplicates_invoices_and_includes_only_nonblank_reasons()
    {
        var errors = new List<BillingErrorDto>
        {
            new("TST", "1", "Shop", "Rejected"),
            new("TST", "1", "Shop", "Duplicate"),
            new("TST", "2", "Shop", " ")
        };

        var message = FailedBillingMessageBuilder.MessageBuilder(errors, "billing@example.invalid");

        Assert.Equal("billing@example.invalid", message.To);
        Assert.Contains("TST1", message.Body!.Message);
        Assert.Contains("TST2", message.Body.Message);
        Assert.Contains("Total: 2 factura(s)", message.Body.Message);
        Assert.Contains("Error: Rejected", message.Body.Message);
        Assert.DoesNotContain("Duplicate", message.Body.Message);
    }

    [Fact]
    public async Task Validation_behavior_calls_next_when_valid_and_throws_before_next_when_invalid()
    {
        var behavior = new ValidationBehaviour<TestRequest, string>([new TestRequestValidator()]);
        var nextCalls = 0;
        RequestHandlerDelegate<string> next = _ =>
        {
            nextCalls++;
            return Task.FromResult("handled");
        };

        Assert.Equal("handled", await behavior.Handle(new TestRequest("ok"), next, CancellationToken.None));
        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(new TestRequest(""), next, CancellationToken.None));
        Assert.Equal(1, nextCalls);
    }

    private sealed record TestRequest(string Name);
    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator() => RuleFor(x => x.Name).NotEmpty();
    }
}
