using Poliedro.Billing.Application.Billing.Commands.CreateBilling;
using Poliedro.Billing.Application.Billing.Dtos;

namespace Poliedro.Billing.Api.Tests;

public sealed class BillingRequestValidatorTests
{
    [Fact]
    public void Billing_validator_requires_a_collection_with_numbered_invoices()
    {
        var validator = new CreateBillingValidator();

        Assert.False(validator.Validate(new CreateBillingCommand(3, null!)).IsValid);
        Assert.False(validator.Validate(new CreateBillingCommand(3, [])).IsValid);
        Assert.False(validator.Validate(new CreateBillingCommand(3, [Invoice("")])).IsValid);
        Assert.True(validator.Validate(new CreateBillingCommand(3, [Invoice("INV-1")])).IsValid);
    }

    private static CreateBillingDTO Invoice(string number) => new(
        DateTime.Today, DateTime.Now, null, null, null, number, null, null, null, null,
        null, null, null, null, null, null, null, null, null, null, null, null, null, null,
        0, 0, 0, 0, 0, null, null, 0);
}
