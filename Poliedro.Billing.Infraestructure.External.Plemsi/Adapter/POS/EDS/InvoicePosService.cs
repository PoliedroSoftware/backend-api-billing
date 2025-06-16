using MySqlConnector;
using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class InvoicePosService : IInvoicePos
{
    public async Task<IEnumerable<InvoiceEntity>> GetInvoicesAsync(string connectionString, CancellationToken cancellationToken, ClientEntity client)
    {

        List<InvoiceEntity> invoicesFiltraded = [];
        using MySqlConnection connection = new(connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
            string query = $"SELECT v.* FROM v_api_invoice v LEFT JOIN invoice_success i ON v.Resolution = i.verify WHERE i.verify IS NULL AND v.date >= {client.Date.ToShortDateString()} and resolutionType= 'POS' ORDER BY v.number DESC;";
            MySqlCommand command = new(query, connection);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                InvoiceEntity invoice = new()
                {
                    Id = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Time = reader.GetTimeSpan(2),
                    Resolution = reader.GetString(3),
                    Prefix = reader.GetString(4),
                    Number = reader.GetString(5),
                    ResolutionType = reader.GetString(6),
                    Note = reader.GetString(7),
                    AllowanceTotal = reader.GetDecimal(8),
                    InvoiceBaseTotal = reader.GetDecimal(9),
                    InvoiceTaxExclusiveTotal = reader.GetDecimal(10),
                    InvoiceTaxInclusiveTotal = reader.GetDecimal(11),
                    TotalToPay = reader.GetDecimal(12),
                    FinalTotalToPay = reader.GetDecimal(13)
                };
                invoicesFiltraded.Add(invoice);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        return invoicesFiltraded.OrderBy(x => x.Number);
    }
}
