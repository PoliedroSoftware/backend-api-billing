using Moq;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;
using PosItems = Poliedro.Billing.Domain.Billing.ItemsInvoiceEntity;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiItemMappingTests
{
    [Fact]
    public async Task Fe_invoice_item_mapper_calculates_tax_and_line_totals()
    {
        var source = new ItemsInvoiceEntity
        {
            code = 123, description = "Servicio", invoiced_quantity = 3,
            unit_preci = 50, percent = 19, tax_amount = 9.5
        };
        var getItems = new Mock<IGetItemFE>();
        getItems.Setup(service => service.GetItemsAsync("INV-1", "test-connection"))
            .ReturnsAsync([source]);

        var result = await new GetItemsInvoiceFERetail(getItems.Object)
            .GetItemsInvoiceFERetailAsync("INV-1", "test-connection");

        var item = Assert.Single(result);
        var tax = Assert.Single(item.TaxTotals!);
        Assert.Equal(70, item.UnitMeasureId);
        Assert.Equal(150, item.LineExtensionAmount);
        Assert.Equal(3, item.InvoicedQuantity);
        Assert.Equal(50, item.PriceAmount);
        Assert.Equal(28.5, tax.TaxAmount);
        Assert.Equal(150, tax.TaxableAmount);
        Assert.Equal("Servicio IVA 9.5", item.Description);
    }

    [Fact]
    public async Task Fe_invoice_item_mapper_returns_empty_list_without_database_query_when_there_are_no_items()
    {
        var getItems = new Mock<IGetItemFE>();
        getItems.Setup(service => service.GetItemsAsync("INV-EMPTY", "local-only")).ReturnsAsync([]);

        var result = await new GetItemsInvoiceFERetail(getItems.Object)
            .GetItemsInvoiceFERetailAsync("INV-EMPTY", "local-only");

        Assert.Empty(result);
    }

    [Fact]
    public async Task Pos_invoice_item_mapper_preserves_amount_and_applies_provider_contract_defaults()
    {
        var source = new PosItems { Description = "Combustible", Code = "987", InvoicedQuantity = 4, Subtotal = 800, PriceAmount = 200 };
        var getItems = new Mock<IGetItemPos>();
        getItems.Setup(service => service.GetItemsAsync(77, "isolated-connection")).ReturnsAsync([source]);

        var result = await new GetItemsInvoicePos(getItems.Object)
            .GetItemsInvoicePosAsync(77, "isolated-connection");

        var item = Assert.Single(result);
        Assert.Equal(70, item.unit_measure_id);
        Assert.Equal(4, item.invoiced_quantity);
        Assert.Equal(800, item.line_extension_amount);
        Assert.Equal("Combustible", item.description);
        Assert.Equal("987", item.code);
        Assert.Equal("200", item.price_amount);
        Assert.Equal(4, item.base_quantity);
        Assert.Empty(item.tax_totals);
    }
}
