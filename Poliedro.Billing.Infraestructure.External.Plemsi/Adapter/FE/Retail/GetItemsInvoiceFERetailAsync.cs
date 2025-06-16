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
                unit_measure_id = 70,
                line_extension_amount = index.unit_preci * index.invoiced_quantity,
                invoiced_quantity = index.invoiced_quantity,
                free_of_charge_indicator = false,
                allowance_charges = [],

                tax_totals = [
                    new TaxTotalEntity {
                    tax_id = 1,
                    percent = index.percent,
                    tax_amount = index.tax_amount * index.invoiced_quantity,
                    taxable_amount = index.unit_preci * index.invoiced_quantity
                }],
                with_holding_tax_total = [],
                description = $"{index.description} {(index.tax_amount > 0 ? $"IVA {index.tax_amount}" : "")}",

                notes = "",
                code = index.code.ToString(),
                type_item_identification_id = 1,
                price_amount = index.unit_preci,
                base_quantity = index.invoiced_quantity,
            };
            itemsInvoiceResponse.Add(itemInvoice);
        }
        return itemsInvoiceResponse;
    }
}
