using MySqlConnector;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;

public class InvoicesPendingWithDetailsFERepository(IDatabaseUtils databaseUtils) : IInvoicesPendingWithDetailsStrategy
{
    public async Task<IEnumerable<CreateBilling>> GetAllInvoicePendingWithDetails(
    ServerEntity _server,
    DianResolutionEntity _dianResolutionEntity,
    CancellationToken cancellationToken)
    {
        var invoicesMap = new Dictionary<int, CreateBilling>();
        using MySqlConnection connection = new(databaseUtils.GetConnectionString(_server));

        try
        {
            await connection.OpenAsync(cancellationToken);
            string invoicesQuery = @"
            SELECT 
                v.id AS invoice_id,
                v.identication,
                v.person_type,
                v.contact_name,
                v.email,
                v.mobile,
v.address_line_1,
                v.city,
                v.state,
                v.country,
v.custom_field1,
v.custom_field2,
                v.invoice,
                v.payment_status,
                v.transaction_date,
                v.total_before_tax,
                v.discount_amount_by_invoice,
                v.discount_type,
                v.allowanceTotal,
                v.invoiceBaseTotal,
                v.invoiceTaxExclusiveTotal,
                v.invoiceTaxInclusiveTotal,
                v.totalToPay,
                v.send_dian
            FROM v_invoice v
            LEFT JOIN invoice_success i ON v.invoice = i.verify
            WHERE i.verify IS NULL
              AND v.transaction_date >= @date
              AND v.totalToPay <> 0"
                + ((Automatic)_dianResolutionEntity.Automatic == Automatic.No ? " AND v.send_dian = 1 " : "")
                + " ORDER BY v.id ASC";

            using (var cmdInvoices = new MySqlCommand(invoicesQuery, connection))
            {
                cmdInvoices.Parameters.AddWithValue("@date", _dianResolutionEntity.ResolutionDate.ToString("yyyy-MM-dd"));

                using var reader = await cmdInvoices.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    int invoiceId = reader.GetInt32("invoice_id");

                    var invoice = new CreateBilling
                    {
                        Number = reader["invoice"]?.ToString(),

                        TransactionDate = reader.IsDBNull(reader.GetOrdinal("transaction_date"))
                            ? DateTime.MinValue
                            : reader.GetDateTime("transaction_date"),

                        TotalBeforeTax = reader.IsDBNull(reader.GetOrdinal("total_before_tax")) ? 0L : Convert.ToInt64(reader["total_before_tax"]),
                        DiscountAmountByInvoice = reader.IsDBNull(reader.GetOrdinal("discount_amount_by_invoice")) ? 0L : Convert.ToInt64(reader["discount_amount_by_invoice"]),
                        DiscountType = reader["discount_type"]?.ToString(),
                        AllowanceTotal = reader.IsDBNull(reader.GetOrdinal("allowanceTotal")) ? 0L : Convert.ToInt64(reader["allowanceTotal"]),
                        InvoiceBaseTotal = reader.IsDBNull(reader.GetOrdinal("invoiceBaseTotal")) ? 0L : Convert.ToInt64(reader["invoiceBaseTotal"]),
                        InvoiceTaxExclusiveTotal = reader.IsDBNull(reader.GetOrdinal("invoiceTaxExclusiveTotal")) ? 0L : Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                        InvoiceTaxInclusiveTotal = reader.IsDBNull(reader.GetOrdinal("invoiceTaxInclusiveTotal")) ? 0L : Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                        TotalToPay = reader.IsDBNull(reader.GetOrdinal("totalToPay")) ? 0L : Convert.ToInt64(reader["totalToPay"]),

                        CustomerEntity = new CustomerEntity
                        {
                            IdentificationNumber = reader["identication"]?.ToString(),
                            TypeOfPerson = reader.IsDBNull(reader.GetOrdinal("person_type")) ? 0 : Convert.ToInt32(reader["person_type"]),
                            Name = reader["contact_name"]?.ToString(),
                            Email = reader["email"]?.ToString(),
                            Phone = reader["mobile"]?.ToString(),
                            Address = reader["address_line_1"]?.ToString(),
                            City = reader["city"]?.ToString(),
                            State = reader["state"]?.ToString(),
                            Country = reader["country"]?.ToString(),
                            MunicipalityCode = reader["custom_field1"]?.ToString(),
                            MunicipalityId = reader["custom_field2"]?.ToString()
                        },

                        PaymentEntity = new PaymentEntity
                        {
                            PaymentFormId = reader.IsDBNull(reader.GetOrdinal("payment_status")) ? 0 : Convert.ToInt32(reader["payment_status"])

                        },

                        ItemElectronicEntity = new List<ItemElectronicEntity>()
                    };

                    invoicesMap[invoiceId] = invoice;
                }
            }


            if (invoicesMap.Count == 0)
                return invoicesMap.Values.ToList();


            var invoiceIds = invoicesMap.Keys.ToList();
            const int chunkSize = 1000;
            for (int i = 0; i < invoiceIds.Count; i += chunkSize)
            {
                var chunk = invoiceIds.Skip(i).Take(chunkSize).ToList();


                var paramNames = chunk.Select((id, idx) => $"@id{idx}").ToList();
                string inClause = string.Join(", ", paramNames);

                string detailsQuery = $@"
                SELECT 
                    d.transaccion,
                    d.code,
                    d.type_item_identification_id,
                    d.description,
                    d.unit_measure_id,
                    d.base_quantity,
                    d.invoiced_quantity,
                    d.price_amount,
                    d.line_extension_amount,
                    d.unit_price_before_discount,
                    d.line_discount_amount,
                    d.line_discount_type,
                    d.percent,
                    d.tax_amount,
                    d.unit_price
                FROM v_invoice_detail d
                WHERE d.transaccion IN ({inClause})
                ORDER BY d.transaccion, d.id ASC";

                using var cmdDetails = new MySqlCommand(detailsQuery, connection);
                for (int j = 0; j < chunk.Count; j++)
                {
                    cmdDetails.Parameters.AddWithValue(paramNames[j], chunk[j]);
                }

                using var readerDetails = await cmdDetails.ExecuteReaderAsync(cancellationToken);
                while (await readerDetails.ReadAsync(cancellationToken))
                {
                    int transaccion = readerDetails.GetInt32("transaccion");

                    if (!invoicesMap.TryGetValue(transaccion, out var invoice))
                    {

                        continue;
                    }

                    var item = new ItemElectronicEntity
                    {
                        Transaccion = readerDetails.IsDBNull(readerDetails.GetOrdinal("transaccion")) ? 0 : readerDetails.GetInt32("transaccion"),
                        Code = readerDetails.IsDBNull(readerDetails.GetOrdinal("code")) ? 0 : readerDetails.GetInt32("code"),
                        TypeItemIdentificationId = readerDetails.IsDBNull(readerDetails.GetOrdinal("type_item_identification_id")) ? 0 : readerDetails.GetInt32("type_item_identification_id"),
                        Description = readerDetails["description"]?.ToString(),
                        UnitMeasureId = readerDetails.IsDBNull(readerDetails.GetOrdinal("unit_measure_id")) ? 0 : readerDetails.GetInt32("unit_measure_id"),
                        BaseQuantity = readerDetails.IsDBNull(readerDetails.GetOrdinal("base_quantity")) ? 0.0 : readerDetails.GetDouble("base_quantity"),
                        InvoicedQuantity = readerDetails.IsDBNull(readerDetails.GetOrdinal("invoiced_quantity")) ? 0.0 : readerDetails.GetDouble("invoiced_quantity"),
                        PriceAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("price_amount")) ? 0.0 : readerDetails.GetDouble("price_amount"),
                        LineExtensionAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("line_extension_amount")) ? 0.0 : readerDetails.GetDouble("line_extension_amount"),
                        UnitPriceBeforeDiscount = readerDetails.IsDBNull(readerDetails.GetOrdinal("unit_price_before_discount")) ? 0.0 : readerDetails.GetDouble("unit_price_before_discount"),
                        LineDiscountAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("line_discount_amount")) ? 0.0 : readerDetails.GetDouble("line_discount_amount"),
                        LineDiscountType = readerDetails["line_discount_type"]?.ToString(),
                        Percent = readerDetails.IsDBNull(readerDetails.GetOrdinal("percent")) ? 0.0 : readerDetails.GetDouble("percent"),
                        TaxAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("tax_amount")) ? 0.0 : readerDetails.GetDouble("tax_amount"),
                        UnitPrice = readerDetails.IsDBNull(readerDetails.GetOrdinal("unit_price")) ? 0.0 : readerDetails.GetDouble("unit_price")
                    };

                    invoice.ItemElectronicEntity!.Add(item);
                }
            }

            return invoicesMap.Values.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error connecting to the database", ex);
        }
    }


}
