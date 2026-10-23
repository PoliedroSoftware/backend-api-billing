using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.Common.Methods.Billing.Preparation.ElectronicBilling;

namespace Poliedro.Billing.Domain.Test;

public sealed class BillingDomainRulesTests
{
    private readonly CalculateCheckDigitsBilling _checkDigits = new();
    private readonly ValidateScriptBilling _documentType = new();

    [Theory]
    [InlineData("900336664", "8")]
    [InlineData("1064719869", "4")]
    [InlineData("123456789", "6")]
    public async Task CalculateCheckDigit_ReturnsExpectedModuloElevenDigit(string identification, string expected)
    {
        var result = await _checkDigits.CalculateCheckDigit(identification, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ABC123")]
    [InlineData("12-345")]
    public async Task CalculateCheckDigit_ReturnsErrorForNonNumericIdentification(string identification)
    {
        var result = await _checkDigits.CalculateCheckDigit(identification, CancellationToken.None);

        Assert.Equal("error", result);
    }

    [Theory]
    [InlineData("900336664-8", DocumentType.NIT)]
    [InlineData("1064719869", DocumentType.CC)]
    public async Task ValidateScript_ClassifiesIdentificationByHyphen(string identification, DocumentType expected)
    {
        var result = await _documentType.ValidateScriptAsync(identification, CancellationToken.None);

        Assert.Equal(expected, result);
    }
}
