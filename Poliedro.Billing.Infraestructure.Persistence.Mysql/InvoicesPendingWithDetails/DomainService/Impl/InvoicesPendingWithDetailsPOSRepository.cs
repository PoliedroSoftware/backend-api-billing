
using MySqlConnector;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;
public class InvoicesPendingWithDetailsPOSRepository(IDatabaseUtils databaseUtils) : IInvoicesPendingWithDetailsStrategy
{
    public async Task<IEnumerable<CreateBilling>> GetAllInvoicePendingWithDetails(
    ServerEntity _server,
    DianResolutionEntity _dianResolutionEntity,
    CancellationToken cancellationToken)
    {
        var dianResolution = _dianResolutionEntity;

        var invoicesMap = new Dictionary<int, CreateBilling>();
        using MySqlConnection connection = new(databaseUtils.GetConnectionString(_server));

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
            ORDER BY v.number ASC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", dianResolution.ResolutionDate.ToString("yyyy-MM-dd"));
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                int invoiceId = reader.GetInt32("invoice_id");

                if (!invoicesMap.TryGetValue(invoiceId, out var invoice))
                {
                    var number = reader.IsDBNull(reader.GetOrdinal("number")) ? null : reader.GetString("number");

                    if (string.IsNullOrEmpty(number))
                        continue;

                    invoice = new CreateBilling
                    {
                        Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? DateTime.MinValue : reader.GetDateTime("Date"),
                        // Time is intentionally not mapped: the column is a MySQL TIME (TimeSpan) while the
                        // model expects DateTime, and this temporal endpoint does not consume Time (the FE
                        // strategy omits it as well).
                        Number = number,
                        Prefix = reader.IsDBNull(reader.GetOrdinal("prefix")) ? null : reader["prefix"].ToString(),
                        Resolution = reader.IsDBNull(reader.GetOrdinal("Resolution")) ? null : reader["Resolution"].ToString(),
                        Notes = reader.IsDBNull(reader.GetOrdinal("note")) ? null : reader["note"].ToString(),
                        AllowanceTotal = reader.IsDBNull(reader.GetOrdinal("allowanceTotal")) ? 0L : Convert.ToInt64(reader["allowanceTotal"]),
                        InvoiceBaseTotal = reader.IsDBNull(reader.GetOrdinal("invoiceBaseTotal")) ? 0L : Convert.ToInt64(reader["invoiceBaseTotal"]),
                        InvoiceTaxExclusiveTotal = reader.IsDBNull(reader.GetOrdinal("invoiceTaxExclusiveTotal")) ? 0L : Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                        InvoiceTaxInclusiveTotal = reader.IsDBNull(reader.GetOrdinal("invoiceTaxInclusiveTotal")) ? 0L : Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                        TotalToPay = reader.IsDBNull(reader.GetOrdinal("totalToPay")) ? 0L : Convert.ToInt64(reader["totalToPay"]),
                        ItemElectronicEntity = new List<ItemElectronicEntity>()
                    };

                    invoicesMap[invoiceId] = invoice;

                }

                var item = new ItemElectronicEntity
                {
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader["description"].ToString(),
                    Code = int.TryParse(reader["code"]?.ToString(), out var codeValue) ? codeValue : (int?)null,
                    BaseQuantity = reader.IsDBNull(reader.GetOrdinal("base_quantity")) ? 0.0 : reader.GetDouble("base_quantity"),
                    InvoicedQuantity = reader.IsDBNull(reader.GetOrdinal("invoiced_quantity")) ? 0.0 : reader.GetDouble("invoiced_quantity"),
                    PriceAmount = reader.IsDBNull(reader.GetOrdinal("price_amount")) ? 0.0 : reader.GetDouble("price_amount"),
                    Subtotal = reader.IsDBNull(reader.GetOrdinal("subtotal")) ? 0.0 : reader.GetDouble("subtotal")
                };

                invoice.ItemElectronicEntity!.Add(item);
            }

            return invoicesMap.Values.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error connecting to the database for resolution {_dianResolutionEntity.ResolutionId} and server {_server.ServerId} ({_server.Ip}/{_server.DatabaseName})", ex);
        }
    }
}
