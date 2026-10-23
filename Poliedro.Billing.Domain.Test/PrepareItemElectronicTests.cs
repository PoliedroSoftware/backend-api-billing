using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Common.Methods.Billing.Preparation.ElectronicBilling;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Test;

public class PrepareItemElectronicTests
{
    private readonly PrepareItemElectronic _prepareItem = new();

    [Fact]
    public async Task Item_without_any_price_throws_and_reports_the_item()
    {
        var items = new List<ItemElectronicEntity>
        {
            new()
            {
                Code = 403,
                Description = "MUESTRA CICA CALMING PWDER WASH TOCOBO",
                InvoicedQuantity = 2,
                UnitPrice = 0,
                UnitPriceBeforeDiscount = 0,
                PriceAmount = 0,
                LineExtensionAmount = 0
            }
        };

        var exception = await Assert.ThrowsAsync<ItemPriceNotInformedException>(
            () => _prepareItem.PrepareItemBillingAsync(items));

        Assert.Contains("403", exception.Message);
        Assert.Contains("MUESTRA", exception.Message);
    }

    [Fact]
    public async Task Free_item_takes_the_reference_price_from_unit_price_before_discount()
    {
        var items = new List<ItemElectronicEntity>
        {
            new()
            {
                Code = 403,
                Description = "MUESTRA",
                InvoicedQuantity = 2,
                UnitPrice = 0,
                UnitPriceBeforeDiscount = 26000,
                PriceAmount = 0,
                LineExtensionAmount = 0
            }
        };

        var result = await _prepareItem.PrepareItemBillingAsync(items);

        var item = Assert.Single(result);
        Assert.True(item.FreeOfChargeIndicator);
        Assert.Equal(0, item.LineExtensionAmount);
        Assert.Equal(26000, item.PriceAmount);
        Assert.Empty(item.AllowanceCharges!);
        Assert.Contains("ENTREGA GRATUITA", item.Description);
    }

    [Fact]
    public async Task Free_item_takes_the_reference_price_from_price_amount()
    {
        var items = new List<ItemElectronicEntity>
        {
            new()
            {
                Code = 403,
                InvoicedQuantity = 1,
                UnitPrice = 0,
                UnitPriceBeforeDiscount = 0,
                PriceAmount = 15000,
                LineExtensionAmount = 0
            }
        };

        var result = await _prepareItem.PrepareItemBillingAsync(items);

        Assert.Equal(15000, Assert.Single(result).PriceAmount);
    }

    [Fact]
    public async Task Paid_item_gets_the_gross_price_as_reference()
    {
        var items = new List<ItemElectronicEntity>
        {
            new()
            {
                Code = 295,
                InvoicedQuantity = 1,
                UnitPrice = 100000,
                UnitPriceBeforeDiscount = 105000,
                PriceAmount = 0,
                LineExtensionAmount = 100000,
                LineDiscountAmount = 5000,
                LineDiscountType = "fixed"
            }
        };

        var result = await _prepareItem.PrepareItemBillingAsync(items);

        var item = Assert.Single(result);
        Assert.False(item.FreeOfChargeIndicator);
        Assert.Equal(105000, item.PriceAmount);
        Assert.Equal(100000, item.LineExtensionAmount);

        var allowance = Assert.Single(item.AllowanceCharges!);
        Assert.Equal(5000, (double)allowance.Amount);
        Assert.Equal(105000, (double)allowance.BaseAmount);
    }

    [Fact]
    public async Task Paid_item_without_unit_price_rebuilds_it_from_the_line_value()
    {
        var items = new List<ItemElectronicEntity>
        {
            new()
            {
                Code = 1,
                InvoicedQuantity = 1,
                UnitPrice = 0,
                UnitPriceBeforeDiscount = 0,
                PriceAmount = 0,
                LineExtensionAmount = 26000
            }
        };

        var result = await _prepareItem.PrepareItemBillingAsync(items);

        var item = Assert.Single(result);
        Assert.False(item.FreeOfChargeIndicator);
        Assert.Equal(26000, item.PriceAmount);
        Assert.Equal(26000, item.LineExtensionAmount);
    }

    [Fact]
    public async Task Tax_totals_use_item_rate_when_source_tax_rate_is_missing_and_apply_tax_mapping()
    {
        var items = new List<ItemElectronicEntity>
        {
            new()
            {
                Code = 7, Description = "Producto gravado", InvoicedQuantity = 1,
                UnitPrice = 100, PriceAmount = 100, LineExtensionAmount = 100, Percent = 19,
                TaxTotals = [new() { TaxId = 1, Percent = 0, TaxAmount = 0, TaxableAmount = 0 }]
            }
        };

        var item = Assert.Single(await _prepareItem.PrepareItemBillingAsync(items));
        var tax = Assert.Single(item.TaxTotals!);

        Assert.Equal(1, tax.TaxId);
        Assert.Equal(19, tax.Percent);
        Assert.Equal(19, tax.TaxAmount);
        Assert.Equal(100, tax.TaxableAmount);
        Assert.Contains("IVA 19", item.Description);
    }

    [Fact]
    public async Task Unknown_internal_tax_id_is_preserved_for_provider_adapter_mapping()
    {
        var item = Assert.Single(await _prepareItem.PrepareItemBillingAsync(
        [new ItemElectronicEntity
        {
            Code = 8, Description = "IVA", InvoicedQuantity = 1, UnitPrice = 100,
            PriceAmount = 100, LineExtensionAmount = 100, Percent = 19,
            TaxTotals = [new() { TaxId = 999, Percent = 19 }]
        }]));

        Assert.Equal(999, Assert.Single(item.TaxTotals!).TaxId);
        Assert.Equal(19, item.TaxTotals[0].TaxAmount);
    }

    [Theory]
    [InlineData("fijo", 100, true)]
    [InlineData("porcentaje", 100, true)]
    [InlineData("porcentáje", 25, false)]
    public async Task Localized_discount_types_are_normalized_and_full_discount_becomes_free(
        string discountType, double discountValue, bool expectedFree)
    {
        var item = Assert.Single(await _prepareItem.PrepareItemBillingAsync(
        [new ItemElectronicEntity
        {
            Code = 9, Description = "Descuento", InvoicedQuantity = 1,
            UnitPriceBeforeDiscount = 100, UnitPrice = 100, PriceAmount = 100,
            LineExtensionAmount = 100, LineDiscountType = discountType,
            LineDiscountAmount = discountValue, Percent = 0
        }]));

        Assert.Equal(expectedFree, item.FreeOfChargeIndicator);
        Assert.Equal(expectedFree ? 0 : 75, item.LineExtensionAmount);
        if (expectedFree)
            Assert.Empty(item.AllowanceCharges!);
        else
            Assert.Equal(25, (double)Assert.Single(item.AllowanceCharges!).Amount);
    }

    [Fact]
    public async Task Free_item_without_source_tax_has_no_tax_and_uses_default_unit_note_and_identification()
    {
        var item = Assert.Single(await _prepareItem.PrepareItemBillingAsync(
        [new ItemElectronicEntity
        {
            Code = 10, Description = "Muestra", InvoicedQuantity = 2,
            UnitPriceBeforeDiscount = 25, LineExtensionAmount = 0,
            UnitMeasureId = 0, Notes = "  ", TaxTotals = []
        }]));

        Assert.True(item.FreeOfChargeIndicator);
        Assert.Equal(70, item.UnitMeasureId);
        Assert.Equal("Description", item.Notes);
        Assert.Equal(1, item.TypeItemIdentificationId);
        Assert.Empty(item.TaxTotals!);
        Assert.Equal(25, item.PriceAmount);
        Assert.Equal(2, item.InvoicedQuantity);
    }

    [Fact]
    public async Task Fixed_discount_equal_to_gross_amount_creates_free_item()
    {
        var item = Assert.Single(await _prepareItem.PrepareItemBillingAsync(
        [new ItemElectronicEntity
        {
            Code = 11, Description = "Obsequio", InvoicedQuantity = 1,
            UnitPriceBeforeDiscount = 250, UnitPrice = 250, LineExtensionAmount = 250,
            LineDiscountType = "amount", LineDiscountAmount = 250
        }]));

        Assert.True(item.FreeOfChargeIndicator);
        Assert.Equal(250, item.PriceAmount);
        Assert.Equal(0, item.LineExtensionAmount);
    }

    [Fact]
    public async Task A_negative_net_amount_is_clamped_to_zero_and_marked_free()
    {
        var item = Assert.Single(await _prepareItem.PrepareItemBillingAsync(
        [new ItemElectronicEntity
        {
            Code = 12, Description = "Ajuste", InvoicedQuantity = 1,
            UnitPriceBeforeDiscount = 100, UnitPrice = -10, LineExtensionAmount = -10
        }]));

        Assert.True(item.FreeOfChargeIndicator);
        Assert.Equal(0, item.LineExtensionAmount);
    }
}
