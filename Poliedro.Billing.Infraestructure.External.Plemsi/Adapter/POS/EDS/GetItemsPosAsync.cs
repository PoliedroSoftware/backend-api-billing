using MySqlConnector;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS
{
    public class GetItem : IGetItemPos
    {
        public async Task<List<ItemsInvoiceEntity>> GetItemsAsync(int invoiceItem, string connectionString)
        {
            List<ItemsInvoiceEntity> invoices = new();
            using (MySqlConnection connection = new(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM v_api_item WHERE resolution = " +
                                   "(SELECT resolution FROM v_api_invoice v WHERE RIGHT(v.number, 6) = @InvoiceItem Limit 1)";

                    using MySqlCommand command = new(query, connection);
                    command.Parameters.AddWithValue("@InvoiceItem", invoiceItem);

                    using MySqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetDecimal(5) > 0)
                        {
                            ItemsInvoiceEntity invoice = new()
                            {
                                Resolution = reader.GetString(0),
                                Description = reader.GetString(1),
                                Code = reader.GetString(2),
                                BaseQuantity = reader.GetDecimal(3),
                                InvoicedQuantity = reader.GetDecimal(4),
                                PriceAmount = reader.GetDecimal(5),
                                Subtotal = reader.GetDecimal(6)
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
}
