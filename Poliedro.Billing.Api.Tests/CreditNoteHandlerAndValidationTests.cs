using AutoMapper;
using FluentValidation.TestHelper;
using Moq;
using Poliedro.Billing.Application.CreditNote.Commands.CreateCreditNote;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.CreditNote.Ports;
using Poliedro.Billing.Domain.Ports;
using CreditNoteEntity = Poliedro.Billing.Domain.CreditNote.Entity.CreditNoteEntity;
using ApiResponseCreditNote = Poliedro.Billing.Domain.CreditNote.Entity.ApiResponseCreditNote;

namespace Poliedro.Billing.Api.Tests;

public sealed class CreditNoteHandlerAndValidationTests
{
    [Fact]
    public async Task Handler_maps_request_and_forwards_domain_result_and_cancellation()
    {
        var command = ValidCommand();
        var entity = new CreditNoteEntity();
        var expected = Result<ApiResponseCreditNote, Error>.Failure(Error.CreateInstance("test", "provider rejected", System.Net.HttpStatusCode.BadRequest));
        var mapper = new Mock<IMapper>(MockBehavior.Strict);
        mapper.Setup(x => x.Map<CreditNoteEntity>(command)).Returns(entity);
        using var cancellation = new CancellationTokenSource();
        var domain = new Mock<ICreditNoteDomainService>(MockBehavior.Strict);
        domain.Setup(x => x.Create(entity, cancellation.Token)).ReturnsAsync(expected);
        var handler = new CreateCreditNoteCommandHandler(domain.Object, mapper.Object);

        var result = await handler.Handle(command, cancellation.Token);

        Assert.Same(expected, result);
        mapper.VerifyAll();
        domain.VerifyAll();
    }

    [Fact]
    public async Task Command_validator_rejects_missing_discrepancy_resolution_prefix_and_nonpositive_number()
    {
        var validator = new CreateCreditNoteCommandValidator(MessageProvider());
        var result = await validator.TestValidateAsync(ValidCommand() with
        {
            Discrepancy = null!,
            Resolution = "",
            Prefix = "",
            Number = 0
        });

        result.ShouldHaveValidationErrorFor(x => x.Discrepancy);
        result.ShouldHaveValidationErrorFor(x => x.Resolution);
        result.ShouldHaveValidationErrorFor(x => x.Prefix);
        result.ShouldHaveValidationErrorFor(x => x.Number);
    }

    [Fact]
    public async Task Nested_credit_note_validators_cover_invalid_and_valid_line_and_tax_values()
    {
        var messages = MessageProvider();
        var itemValidator = new ItemValidator(messages);
        var invalidItem = await itemValidator.TestValidateAsync(new ItemElectronicEntity(
            0, 0, false, [], [], "", "", "", 1, 0, 1, 1));
        Assert.True(invalidItem.Errors.Count >= 5);
        var validItem = await itemValidator.TestValidateAsync(new ItemElectronicEntity(
            70, 100, false, [], [], "Servicio", "", "SKU-1", 1, 100, 1, 1));
        Assert.Empty(validItem.Errors);

        var taxValidator = new TaxTotalValidator(messages);
        Assert.True((await taxValidator.TestValidateAsync(new TaxTotal(1, -1, 101, 100))).Errors.Count >= 2);
        Assert.Empty((await taxValidator.TestValidateAsync(new TaxTotal(1, 0, 19, 100))).Errors);
    }

    private static CreateCreditNoteCommand ValidCommand() => new(
        new Discrepancy(1, "Correction"), "R-1", "CN", 1, [],
        new Seller("1", "0", "Seller", "3000000000", "Street", "11001", "s@example.invalid", "0", 1, 1, 1, 1, 1),
        "footer", "header", new InvoiceReference("2026-01-01", "uuid", "FE-1"), [], 0, 0, 0, 0, 0, [], []);

    private static IMessageProvider MessageProvider()
    {
        var mock = new Mock<IMessageProvider>();
        mock.SetupGet(x => x.ErrorValidatorFieldNotNull).Returns("required");
        mock.SetupGet(x => x.ErrorValidatorFieldNotEmpty).Returns("not empty");
        mock.SetupGet(x => x.ErrorValidatorFieldMustBeGreaterThanZero).Returns("positive");
        mock.SetupGet(x => x.ErrorValidatorFieldMustBeNonNegative).Returns("nonnegative");
        mock.SetupGet(x => x.ErrorValidatorFieldMustBePercentage).Returns("percentage");
        return mock.Object;
    }
}
