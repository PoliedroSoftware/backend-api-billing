using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;

public class GetItemsInvoiceFERetail(IGetItemFE getItem) : IGetItemsInvoiceFERetail
{
    public async Task<List<ItemElectronicEntity>> GetItemsInvoiceFERetailAsync(string invoice, string connectionString)
    {
        List<ItemElectronicEntity> itemsInvoiceResponse = new();
        List<ItemsInvoiceEntity> itemsInvoice = await getItem.GetItemsAsync(invoice, connectionString);
        foreach (ItemsInvoiceEntity index in itemsInvoice)
        {
            var itemInvoice = new ItemElectronicEntity
            {
                UnitMeasureId = 70,
                LineExtensionAmount = index.unit_preci * index.invoiced_quantity,
                InvoicedQuantity = index.invoiced_quantity,
                FreeOfChargeIndicator = false,
                AllowanceCharges = [],

                TaxTotals = [
                    new TaxTotalEntity {
                    TaxId = 1,
                    Percent = index.percent,
                    TaxAmount = index.tax_amount * index.invoiced_quantity,
                    TaxableAmount = index.unit_preci * index.invoiced_quantity
                }],
                WithHoldingTaxTotal = [],
                Description = $"{index.description} {(index.tax_amount > 0 ? $"IVA {index.tax_amount}" : "")}",

                Notes = "",
                Code = index.code,
                TypeItemIdentificationId = 1,
                PriceAmount = index.unit_preci,
                BaseQuantity = index.invoiced_quantity,
            };
            itemsInvoiceResponse.Add(itemInvoice);
        }
        return itemsInvoiceResponse;
    }
}
