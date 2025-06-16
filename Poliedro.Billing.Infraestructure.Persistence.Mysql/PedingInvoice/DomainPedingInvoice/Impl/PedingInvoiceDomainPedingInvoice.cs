using MySqlConnector;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
using Poliedro.Billing.Domain.PedingInvoice.DomainPedingInvoice;
using Poliedro.Billing.Domain.PedingInvoice.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.PedingInvoice.DomainPedingInvoice.Impl;

public class PedingInvoiceDomainPedingInvoice() : IPedingInvoiceDomainPedingInvoice
{
    public async Task<(IEnumerable<PedingInvoiceEntity> Data, int TotalCount)> GetAllAsync(
         ServerEntity server,
        IDatabaseUtils databaseUtils,
        DateTime DateTime,
         InvoiceElectronicParameters paginationParams,
        CancellationToken cancellationToken)
    {
        var pendingInvoices = new List<PedingInvoiceEntity>();
        
        using MySqlConnection connection = new(databaseUtils.GetConnectionString(server));

        string orderDirection = paginationParams.OrderBy?.ToLower() == "desc" ? "DESC" : "ASC";
        int skip = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

        try
        {
            await connection.OpenAsync(cancellationToken);

            var countQuery = "SELECT COUNT(DISTINCT id) FROM v_invoice";
            using var countCommand = new MySqlCommand(countQuery, connection);
            var date = DateTime;
            var totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));

            string query = $@"
            SELECT v.* 
            FROM v_invoice v  
            LEFT JOIN invoice_success i ON v.invoice = i.verify 
            WHERE i.verify IS NULL 
            AND v.transaction_date >= @date AND v.send_dian = 1
            ORDER BY v.id {orderDirection}
            LIMIT @Skip, @Take;";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@Skip", skip);
            command.Parameters.AddWithValue("@Take", paginationParams.PageSize);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                PedingInvoiceEntity invoice = new()
                {
                    Id = reader.GetInt32(0),
                    identication = reader.GetString(1),
                    contact_name = reader.GetString(2),
                    email = reader.GetString(3),
                    mobile = reader.GetString(4),
                    city = reader.GetString(5),
                    state = reader.GetString(6),
                    country = reader.GetString(7),
                    invoice = reader.GetString(8),
                    payment_status = reader.GetString(9),
                    transaction_date = reader.IsDBNull(10) ? "" : reader.GetDateTime(10).ToString("yyyy-MM-dd"),
                    allowanceTotal = reader.GetInt64(11),
                    invoiceBaseTotal = reader.GetInt64(12),
                    invoiceTaxExclusiveTotal = reader.GetInt64(13),
                    invoiceTaxInclusiveTotal = reader.GetInt64(14),
                    totalToPay = reader.GetInt64(15),
                };
                pendingInvoices.Add(invoice);
            }
            return (pendingInvoices, totalCount);
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching pending invoice", ex);
        }

    }
}

