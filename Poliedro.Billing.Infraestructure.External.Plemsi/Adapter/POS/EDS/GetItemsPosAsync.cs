using System.Data.Common;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class GetItem(IRelationalConnectionFactory connectionFactory) : IGetItemPos
{
    public async Task<List<ItemsInvoiceEntity>> GetItemsAsync(int invoiceItem, string connectionString)
    {
        var invoices = new List<ItemsInvoiceEntity>();
        await using var connection = connectionFactory.Create(connectionString);
        try
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT * FROM v_api_item WHERE resolution =
                  (SELECT resolution FROM v_api_invoice v WHERE RIGHT(v.number, 6) = @InvoiceItem Limit 1)
                """;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@InvoiceItem";
            parameter.Value = invoiceItem;
            command.Parameters.Add(parameter);

            await using DbDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetDecimal(5) <= 0)
                    continue;

                invoices.Add(new ItemsInvoiceEntity
                {
                    Resolution = reader.GetString(0),
                    Description = reader.GetString(1),
                    Code = reader.GetString(2),
                    BaseQuantity = reader.GetDouble(3),
                    InvoicedQuantity = reader.GetDouble(4),
                    PriceAmount = reader.GetDecimal(5),
                    Subtotal = reader.GetDouble(6)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        return invoices;
    }
}
