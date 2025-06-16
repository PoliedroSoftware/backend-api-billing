using MySqlConnector;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
public class InvoiceFEService : IInvoiceFE
{
    public InvoiceFEService()
    {
    }
    public async Task<IEnumerable<InvoiceEntity>> GetInvoicesAsync(
    string connectionString,
    CancellationToken cancellationToken,
    ClientEntity clientItem)
    {
        List<InvoiceEntity> invoicesFiltraded = new();

        using MySqlConnection connection = new(connectionString);
        try
        {
            
            await connection.OpenAsync(cancellationToken);
            var query = @"
                SELECT v.* 
                FROM v_invoice v  
                LEFT JOIN invoice_success i ON v.invoice = i.verify 
                WHERE i.verify IS NULL 
                  AND v.transaction_date >= @date "
                + ((Automatic)clientItem.Automatic == Automatic.No ? " AND v.send_dian = 1 " : "")
                + "ORDER BY v.id ASC";



            using MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@date", clientItem.Date.ToString("yyyy-MM-dd"));

            using MySqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
               
                if ((Automatic)clientItem.Automatic == Automatic.No)
                {
                    if ((Automatic)Convert.ToInt32(reader["send_dian"]) == Automatic.Yes)
                    {
                        InvoiceEntity invoice = new()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            identication = reader["identication"].ToString(),
                            contact_name = reader["contact_name"].ToString(),
                            email = reader["email"].ToString(),
                            mobile = reader["mobile"].ToString(),
                            city = reader["city"].ToString(),
                            state = reader["state"].ToString(),
                            country = reader["country"].ToString(),
                            invoice = reader["invoice"].ToString(),
                            payment_status = reader["payment_status"].ToString(),
                            transaction_date = reader["transaction_date"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(reader["transaction_date"]).ToString("yyyy-MM-dd"),
                            allowanceTotal = Convert.ToInt64(reader["allowanceTotal"]),
                            invoiceBaseTotal = Convert.ToInt64(reader["invoiceBaseTotal"]),
                            invoiceTaxExclusiveTotal = Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                            invoiceTaxInclusiveTotal = Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                            totalToPay = Convert.ToInt64(reader["totalToPay"]),
                            sendDian = Convert.ToInt32(reader["send_dian"]),
                        };

                        invoicesFiltraded.Add(invoice);
                    }
                }
                else if (Convert.ToInt64(reader["totalToPay"]) > 0)
                {
                    InvoiceEntity invoice = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        identication = reader["identication"].ToString(),
                        contact_name = reader["contact_name"].ToString(),
                        email = reader["email"].ToString(),
                        mobile = reader["mobile"].ToString(),
                        city = reader["city"].ToString(),
                        state = reader["state"].ToString(),
                        country = reader["country"].ToString(),
                        invoice = reader["invoice"].ToString(),
                        payment_status = reader["payment_status"].ToString(),
                        transaction_date = reader["transaction_date"] == DBNull.Value
                             ? ""
                             : Convert.ToDateTime(reader["transaction_date"]).ToString("yyyy-MM-dd"),
                        allowanceTotal = Convert.ToInt64(reader["allowanceTotal"]),
                        invoiceBaseTotal = Convert.ToInt64(reader["invoiceBaseTotal"]),
                        invoiceTaxExclusiveTotal = Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                        invoiceTaxInclusiveTotal = Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                        totalToPay = Convert.ToInt64(reader["totalToPay"]),
                        sendDian = Convert.ToInt32(reader["send_dian"]),
                    };
                    invoicesFiltraded.Add(invoice);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return invoicesFiltraded;
    }


}
