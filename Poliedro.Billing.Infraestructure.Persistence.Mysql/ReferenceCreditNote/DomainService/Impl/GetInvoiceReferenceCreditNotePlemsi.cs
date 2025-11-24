using MySqlConnector;
using Poliedro.Billing.Domain.BillingCreditNote.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.ReferenceCreditNote.DomainService.Impl;

public class GetInvoiceReferenceCreditNotePlemsi(IDatabaseUtils databaseUtils) : IGetInvoiceReferenceCreditNotePlemsi
{
    public async Task<InvoiceReferenceEntity> GetInvoiceReferenceCreditNotePlemsiAsync(
        string Verify,
        ServerEntity server,
        CancellationToken cancellationToken)
    {
        _ = new InvoiceReferenceEntity();

        using MySqlConnection connection = new(databaseUtils.GetConnectionString(server));

        try
        {
            await connection.OpenAsync(cancellationToken);

            var query = $@"SELECT invoice,dateregister,cude FROM invoice_success WHERE verify = {Verify};";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                InvoiceReferenceEntity invoiceReference = new()
                {
                    Number = reader["invoice"] != DBNull.Value ? reader["invoice"].ToString() : null,
                    UuId = reader["cude"] != DBNull.Value ? reader["cude"].ToString() : null,
                    IssueDate = reader["dateregister"] != DBNull.Value ? Convert.ToDateTime(reader["dateregister"]).ToString("yyyy-MM-dd") : null

                };
                return invoiceReference;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching pending invoice", ex);
           
        }
        return null;
    }

}
