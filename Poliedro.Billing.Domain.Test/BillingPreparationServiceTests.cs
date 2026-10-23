using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.Common.Methods.Billing.Preparation.ElectronicBilling;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Test;

public sealed class BillingPreparationServiceTests
{
    [Theory]
    [InlineData("900373913", "4")]
    [InlineData("123", "8")]
    [InlineData("abc", "error")]
    [InlineData("", "error")]
    public async Task Check_digit_service_handles_numeric_and_invalid_identifiers(string value, string expected)
    {
        var result = await new CalculateCheckDigitsBilling().CalculateCheckDigit(value, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("NIT-900373913", DocumentType.NIT)]
    [InlineData("900373913", DocumentType.CC)]
    public async Task Document_type_service_classifies_identifier(string value, DocumentType expected)
    {
        var result = await new ValidateScriptBilling().ValidateScriptAsync(value, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Tax_total_service_groups_by_tax_and_rate_and_sums_amounts()
    {
        var items = new List<ItemElectronicEntity>
        {
            new() { TaxTotals = [new() { TaxId = 1, Percent = 19, TaxAmount = 19, TaxableAmount = 100 }, new() { TaxId = 2, Percent = 0, TaxAmount = 0, TaxableAmount = 50 }] },
            new() { TaxTotals = [new() { TaxId = 1, Percent = 19, TaxAmount = 38, TaxableAmount = 200 }] },
            new() { TaxTotals = null }
        };

        var result = await new GetAllTaxTotalsBilling().IGetAllTaxTotalsBillingAsync(items);

        Assert.Equal(2, result.Count);
        var iva = Assert.Single(result, tax => tax.TaxId == 1);
        Assert.Equal(19, iva.Percent);
        Assert.Equal(57, iva.TaxAmount);
        Assert.Equal(300, iva.TaxableAmount);
        Assert.Equal(0, Assert.Single(result, tax => tax.TaxId == 2).TaxAmount);
    }

    [Fact]
    public async Task Tax_total_service_returns_empty_for_no_tax_details()
    {
        var result = await new GetAllTaxTotalsBilling().IGetAllTaxTotalsBillingAsync([new ItemElectronicEntity { TaxTotals = [] }]);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Item_preparation_returns_empty_for_empty_input()
    {
        var result = await new PrepareItemElectronic().PrepareItemBillingAsync([]);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Paid_item_calculates_tax_and_descriptive_text()
    {
        var result = await new PrepareItemElectronic().PrepareItemBillingAsync(
        [new ItemElectronicEntity { Code = 12, Description = "Producto", InvoicedQuantity = 2, UnitPrice = 100, PriceAmount = 100, Percent = 19, LineExtensionAmount = 200 }]);

        var item = Assert.Single(result);
        Assert.Equal(200, item.LineExtensionAmount);
        Assert.Equal(100, item.PriceAmount);
        Assert.Contains("IVA 38", item.Description);
        Assert.Equal(38, Assert.Single(item.TaxTotals!).TaxAmount);
    }

    [Fact]
    public async Task Percentage_discount_is_applied_to_gross_when_net_price_was_not_reduced()
    {
        var result = await new PrepareItemElectronic().PrepareItemBillingAsync(
        [new ItemElectronicEntity { Code = 3, Description = "Servicio", InvoicedQuantity = 1, UnitPrice = 1000, UnitPriceBeforeDiscount = 1000, LineExtensionAmount = 1000, LineDiscountType = " porcentaje ", LineDiscountAmount = 10, Percent = 0 }]);

        var item = Assert.Single(result);
        Assert.Equal(900, item.LineExtensionAmount);
        Assert.Equal(1000, item.PriceAmount);
        Assert.Equal(100, Assert.Single(item.AllowanceCharges!).Amount);
    }

    [Fact]
    public async Task Fully_discounted_item_becomes_free_and_preserves_reference_tax_as_zero()
    {
        var result = await new PrepareItemElectronic().PrepareItemBillingAsync(
        [new ItemElectronicEntity { Code = 4, Description = "Muestra", InvoicedQuantity = 1, UnitPriceBeforeDiscount = 500, UnitPrice = 500, LineExtensionAmount = 500, LineDiscountType = "percentage", LineDiscountAmount = 100, TaxTotals = [new() { TaxId = 1, Percent = 19, TaxAmount = 95, TaxableAmount = 500 }] }]);

        var item = Assert.Single(result);
        Assert.True(item.FreeOfChargeIndicator);
        Assert.Equal(0, item.LineExtensionAmount);
        var tax = Assert.Single(item.TaxTotals!);
        Assert.Equal(0, tax.TaxAmount);
        Assert.Equal(0, tax.TaxableAmount);
    }
}
