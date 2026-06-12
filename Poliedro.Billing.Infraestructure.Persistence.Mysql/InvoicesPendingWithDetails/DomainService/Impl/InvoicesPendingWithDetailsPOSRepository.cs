
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
                    bool addInvoice = !string.IsNullOrEmpty(reader.GetString("number"));


                    if (!addInvoice)
                        continue;

                    invoice = new CreateBilling
                    {
                        Date = reader.GetDateTime("Date"),
                        //Time = reader.GetTimeSpan("Time"),
                        Number = reader["number"].ToString(),
                        Prefix = reader["prefix"].ToString(),
                        Resolution = reader["Resolution"].ToString(),
                        Notes = reader["note"].ToString(),
                        AllowanceTotal = Convert.ToInt64(reader["allowanceTotal"]),
                        InvoiceBaseTotal = Convert.ToInt64(reader["invoiceBaseTotal"]),
                        InvoiceTaxExclusiveTotal = Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                        InvoiceTaxInclusiveTotal = Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                        TotalToPay = reader.IsDBNull(reader.GetOrdinal("totalToPay")) ? 0L : Convert.ToInt64(reader["totalToPay"]),
                        ItemElectronicEntity = new List<ItemElectronicEntity>()
                    };

                    invoicesMap[invoiceId] = invoice;

                }

                var item = new ItemElectronicEntity
                {
                    Description = reader["description"].ToString(),
                    Code = int.TryParse(reader["code"]?.ToString(), out var codeValue) ? codeValue : (int?)null,
                    BaseQuantity = reader.GetDouble("base_quantity"),
                    InvoicedQuantity = reader.GetDouble("invoiced_quantity"),
                    PriceAmount = reader.GetDouble("price_amount"),
                    Subtotal = reader.GetDouble("subtotal")
                };

                invoice.ItemElectronicEntity!.Add(item);
            }

            return invoicesMap.Values.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error connecting to the database", ex);
        }
    }
}
