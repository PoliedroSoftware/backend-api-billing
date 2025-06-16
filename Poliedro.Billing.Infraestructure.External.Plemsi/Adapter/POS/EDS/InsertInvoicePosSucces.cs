using MySqlConnector;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS
{
    public class InsertInvoice : IInsertInvoicePos
    {
        public async Task InsertInvoiceSucces(
               int invoice,
               string? cude,
               string? QRCode,
               string connectionString,
               ProviderType providerType,
               int ClientBillingElectronicId,
               string invoiceResolution)
        {

            using MySqlConnection connection = new(connectionString);
            try
            {
                await connection.OpenAsync();
                string insertDataQuery = "INSERT INTO invoice_success (invoice, providerid, dateregister, cude, qrCode, client_id, verify) VALUES (@Invoice, @Providerid, @DateRegister, @cude, @qrcode, @client_id, @verify)";
                MySqlCommand insertDataCommand = new(insertDataQuery, connection);
                insertDataCommand.Parameters.AddWithValue("@Invoice", invoice);
                insertDataCommand.Parameters.AddWithValue("@Providerid", providerType.ToString());
                insertDataCommand.Parameters.AddWithValue("@DateRegister", DateTime.Now);
                insertDataCommand.Parameters.AddWithValue("@cude", cude);
                insertDataCommand.Parameters.AddWithValue("@qrcode", QRCode);
                insertDataCommand.Parameters.AddWithValue("@client_id", ClientBillingElectronicId);
                insertDataCommand.Parameters.AddWithValue("@verify", invoiceResolution);
                _ = Task.FromResult(insertDataCommand.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

    }
}
