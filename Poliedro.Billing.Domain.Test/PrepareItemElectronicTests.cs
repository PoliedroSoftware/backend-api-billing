using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;
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
}
