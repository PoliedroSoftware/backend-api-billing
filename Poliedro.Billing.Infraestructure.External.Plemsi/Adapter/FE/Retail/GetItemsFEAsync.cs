using System.Data.Common;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;

public class GetItem(IRelationalConnectionFactory connectionFactory) : IGetItemFE
{
    public async Task<List<ItemsInvoiceEntity>> GetItemsAsync(string invoiceItem, string connectionString)
    {
        var invoices = new List<ItemsInvoiceEntity>();
        await using var connection = connectionFactory.Create(connectionString);
        try
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM v_invoice_detail where transaccion = @InvoiceItem;";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@InvoiceItem";
            parameter.Value = invoiceItem;
            command.Parameters.Add(parameter);

            await using DbDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetDouble(8) <= 0)
                    continue;

                invoices.Add(new ItemsInvoiceEntity
                {
                    id = GetInt(reader, "id"),
                    transaccion = GetInt(reader, "transaccion"),
                    code = GetInt(reader, "code"),
                    type_item_identification_id = GetInt(reader, "type_item_identification_id"),
                    description = GetString(reader, "description"),
                    unit_measure_id = GetInt(reader, "unit_measure_id"),
                    base_quantity = GetDouble(reader, "base_quantity"),
                    invoiced_quantity = GetDouble(reader, "invoiced_quantity"),
                    price_amount = GetDouble(reader, "price_amount"),
                    line_extension_amount = GetDouble(reader, "line_extension_amount"),
                    percent = GetDouble(reader, "percent"),
                    tax_amount = GetDouble(reader, "tax_amount"),
                    unit_preci = GetDouble(reader, "unit_price")
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        return invoices;
    }

    private static int GetInt(DbDataReader reader, string column) => reader.GetInt32(reader.GetOrdinal(column));
    private static double GetDouble(DbDataReader reader, string column) => reader.GetDouble(reader.GetOrdinal(column));
    private static string GetString(DbDataReader reader, string column) => reader.GetString(reader.GetOrdinal(column));
}
