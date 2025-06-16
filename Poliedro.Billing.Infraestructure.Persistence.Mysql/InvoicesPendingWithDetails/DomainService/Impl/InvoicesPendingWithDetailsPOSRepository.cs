
using MySqlConnector;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicePendingWithDetails.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;
public class InvoicesPendingWithDetailsPOSRepository : IInvoicesPendingWithDetailsStrategy
{
    public async Task<IEnumerable<object>> GetAllInvoicePendingWithDetails(
    ServerEntity server,
    ClientEntity clientItem,
    IDatabaseUtils databaseUtils,
    CancellationToken cancellationToken,
    string apiKey)
    {
        var invoicesMap = new Dictionary<int, InvoicePOSPendingEntity>();
        using MySqlConnection connection = new(databaseUtils.GetConnectionString(server));

        try
        {
            await connection.OpenAsync(cancellationToken);

            string query = @"
            SELECT 
                v.id AS invoice_id,
                v.Date,
                v.Time,
                v.Resolution,
                v.prefix,
                v.number,
                v.resolutionType,
                v.note,
                v.allowanceTotal,
                v.invoiceBaseTotal,
                v.invoiceTaxExclusiveTotal,
                v.invoiceTaxInclusiveTotal,
                v.totalToPay,

                d.resolution AS detail_id,
                d.description,
                d.code,
                d.base_quantity,
                d.invoiced_quantity,
                d.price_amount,
                d.subtotal
            FROM v_api_invoice v
            LEFT JOIN invoice_success i ON v.Resolution = i.verify
            LEFT JOIN v_api_item d ON v.Resolution = d.resolution
            WHERE i.verify IS NULL
            AND v.date >= @date
            AND resolutionType= 'POS'
            ORDER BY v.number DESC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", clientItem.Date.ToString("yyyy-MM-dd"));

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                int invoiceId = reader.GetInt32("invoice_id");

                if (!invoicesMap.ContainsKey(invoiceId))
                {
                    bool addInvoice = (reader.GetDecimal(5) > 0);

                    if (!addInvoice)
                        continue;

                    var invoiceEntity = new InvoicePOSPendingEntity
                    {
                        Id = invoiceId,
                        Date = reader.GetDateTime("Date"),
                        Time = reader.GetTimeSpan("Time"),
                        Resolution = reader["Resolution"].ToString(),
                        Prefix = reader["prefix"].ToString(),
                        Number = reader["number"].ToString(),
                        ResolutionType = reader["resolutionType"].ToString(),
                        Note = reader["note"].ToString(),
                        AllowanceTotal = Convert.ToInt64(reader["allowanceTotal"]),
                        InvoiceBaseTotal = Convert.ToInt64(reader["invoiceBaseTotal"]),
                        InvoiceTaxExclusiveTotal = Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                        InvoiceTaxInclusiveTotal = Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                        TotalToPay = Convert.ToInt64(reader["totalToPay"]),
                        DetailsInvoicePendings = new List<DetailsInvoicePOSPendingEntity>()
                    };

                    invoicesMap.Add(invoiceId, invoiceEntity);
                }

                var detail = new DetailsInvoicePOSPendingEntity
                {
                    Id = reader.GetInt32("detail_id"),
                    Description = reader["description"].ToString(),
                    Code = reader["code"].ToString(),
                    BaseQuantity = reader.GetDecimal("base_quantity"),
                    InvoicedQuantity = reader.GetDecimal("invoiced_quantity"),
                    PriceAmount = reader.GetDecimal("price_amount"),
                    Subtotal = reader.GetDecimal("subtotal")
                };

                invoicesMap[invoiceId].DetailsInvoicePendings.Add(detail);
            }

            return invoicesMap.Values.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error connecting to the database", ex);
        }
    }



}
