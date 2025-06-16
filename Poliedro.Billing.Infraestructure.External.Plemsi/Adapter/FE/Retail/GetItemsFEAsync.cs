using MySqlConnector;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;

public class GetItem : IGetItemFE
{
    public async Task<List<ItemsInvoiceEntity>> GetItemsAsync(string invoiceItem, string connectionString)
    {
        List<ItemsInvoiceEntity> invoices = new();
        using (MySqlConnection connection = new(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM v_invoice_detail where transaccion = @InvoiceItem;";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@InvoiceItem", invoiceItem);

                using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if(reader.GetDouble(8) > 0)
                    {
                        ItemsInvoiceEntity invoice = new()
                        {
                            id = reader.GetInt32("id"),
                            transaccion = reader.GetInt32("transaccion"),
                            code = reader.GetInt32("code"),
                            type_item_identification_id = reader.GetInt32("type_item_identification_id"),
                            description = reader.GetString("description"),
                            unit_measure_id = reader.GetInt32("unit_measure_id"),
                            base_quantity = reader.GetDouble("base_quantity"),
                            invoiced_quantity = reader.GetDouble("invoiced_quantity"),
                            price_amount = reader.GetDouble("price_amount"),
                            line_extension_amount = reader.GetDouble("line_extension_amount"),
                            percent = reader.GetDouble("percent"),
                            tax_amount = reader.GetDouble("tax_amount"),
                            unit_preci = reader.GetDouble("unit_price")
                        };
                        invoices.Add(invoice);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        return invoices;
    }

}
