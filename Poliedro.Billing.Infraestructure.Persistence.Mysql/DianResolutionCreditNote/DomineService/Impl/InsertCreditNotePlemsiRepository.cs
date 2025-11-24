using MySqlConnector;
using Poliedro.Billing.Domain.BillingCreditNote.Entities;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolutionCreditNote.DomineService.Impl;

public class InsertCreditNotePlemsiRepository : IInsertCreditNoteRepository
{
    public async Task InsertCreditNoteRepositoryAsync(CreditNoteResultEntity creditNoteResultEntity)
    {

        using MySqlConnection connection = new(creditNoteResultEntity.ConnectionString);

        try
        {
            await connection.OpenAsync();
            string insertDataQuery = "INSERT INTO invoices_success_credit_note (invoice, providerid, dateregister, cude, qrCode, client_id, verify) VALUES (@Invoice, @Providerid, @DateRegister, @cude, @qrcode, @client_id, @verify)";
            MySqlCommand insertDataCommand = new(insertDataQuery, connection);
            insertDataCommand.Parameters.AddWithValue("@Invoice", creditNoteResultEntity.ConsecutiveNumber);
            insertDataCommand.Parameters.AddWithValue("@Providerid", creditNoteResultEntity.ProviderType);
            insertDataCommand.Parameters.AddWithValue("@DateRegister", DateTime.Now);
            insertDataCommand.Parameters.AddWithValue("@cude", creditNoteResultEntity.Cude);
            insertDataCommand.Parameters.AddWithValue("@qrcode", creditNoteResultEntity.QRCode);
            insertDataCommand.Parameters.AddWithValue("@client_id", creditNoteResultEntity.ClientBillingElectronicId);
            insertDataCommand.Parameters.AddWithValue("@verify", creditNoteResultEntity.NumberInvoice);
            _ = Task.FromResult(insertDataCommand.ExecuteNonQuery());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        
    }
}
