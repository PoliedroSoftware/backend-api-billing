using MySqlConnector;
using Poliedro.Billing.Application.Billing.Services.Ports;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Billing
{
    public sealed class MySqlInvoiceEmissionStore : IInvoiceEmissionStore
    {
        public async Task RecordSuccessAsync(
               int invoice,
               string cude,
               string qrCode,
               string connectionString,
               int providerId,
               int clientId,
               string invoiceResolution,
               CancellationToken cancellationToken)
        {
            using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync(cancellationToken);
            string insertDataQuery = "INSERT INTO invoice_success (invoice, providerid, dateregister, cude, qrCode, client_id, verify) VALUES (@Invoice, @Providerid, @DateRegister, @cude, @qrcode, @client_id, @verify)";
            await using MySqlCommand insertDataCommand = new(insertDataQuery, connection);
            insertDataCommand.Parameters.AddWithValue("@Invoice", invoice);
            insertDataCommand.Parameters.AddWithValue("@Providerid", providerId);
            insertDataCommand.Parameters.AddWithValue("@DateRegister", DateTime.Now);
            insertDataCommand.Parameters.AddWithValue("@cude", cude);
            insertDataCommand.Parameters.AddWithValue("@qrcode", qrCode);
            insertDataCommand.Parameters.AddWithValue("@client_id", clientId);
            insertDataCommand.Parameters.AddWithValue("@verify", invoiceResolution);
            await insertDataCommand.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
