using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS
{
    public class GetItemsInvoicePos(IGetItemPos getItem) : IGetItemsInvoicePos

    {
        public async Task<List<ItemFERetailEntity>> GetItemsInvoicePosAsync(int invoice, string connectionString)
        {
            List<ItemFERetailEntity> itemsInvoiceResponse = new();
            List<ItemsInvoiceEntity> itemsInvoice = await getItem.GetItemsAsync(invoice, connectionString);
            foreach (ItemsInvoiceEntity index in itemsInvoice)
            {
                var itemInvoice = new ItemFERetailEntity
                {
                    unit_measure_id = 70,
                    invoiced_quantity = index.InvoicedQuantity.ToString(),
                    line_extension_amount = index.Subtotal.ToString(),
                    free_of_charge_indicator = false,
                    tax_totals =
                    [

                    ],
                    description = index.Description,
                    notes = "xxxx",
                    code = index.Code,
                    type_item_identification_id = 4,
                    price_amount = index.PriceAmount.ToString(),
                    base_quantity = index.InvoicedQuantity.ToString()
                };
                itemsInvoiceResponse.Add(itemInvoice);
            }
            return itemsInvoiceResponse;
        }

    }
}
