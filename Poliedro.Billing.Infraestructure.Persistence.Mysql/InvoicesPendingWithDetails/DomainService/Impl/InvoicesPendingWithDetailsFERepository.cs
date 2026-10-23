using System.Data.Common;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.Entities;
using TaxTotalEntity = Poliedro.Billing.Domain.FERetail.Entity.TaxTotalEntity;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;

public class InvoicesPendingWithDetailsFERepository(IServerDatabaseConnectionFactory connectionFactory) : IInvoicesPendingWithDetailsStrategy
{
    public async Task<IEnumerable<CreateBilling>> GetAllInvoicePendingWithDetails(
    ServerEntity _server,
    DianResolutionEntity _dianResolutionEntity,
    CancellationToken cancellationToken)
    {
        var invoicesMap = new Dictionary<int, CreateBilling>();
        using DbConnection connection = connectionFactory.Create(_server);

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

            using (var cmdInvoices = connection.CreateCommand())
            {
                cmdInvoices.CommandText = invoicesQuery;
                AddParameter(cmdInvoices, "@date", _dianResolutionEntity.ResolutionDate.ToString("yyyy-MM-dd"));

                using var reader = await cmdInvoices.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    int invoiceId = reader.GetInt32(reader.GetOrdinal("invoice_id"));

                    var invoice = new CreateBilling
                    {
                        Number = reader.GetValue(reader.GetOrdinal("invoice"))?.ToString(),

                        TransactionDate = reader.IsDBNull(reader.GetOrdinal("transaction_date"))
                            ? DateTime.MinValue
                            : reader.GetDateTime(reader.GetOrdinal("transaction_date")),

                        TotalBeforeTax = reader.IsDBNull(reader.GetOrdinal("total_before_tax")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("total_before_tax"))),
                        DiscountAmountByInvoice = reader.IsDBNull(reader.GetOrdinal("discount_amount_by_invoice")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("discount_amount_by_invoice"))),
                        DiscountType = reader.GetValue(reader.GetOrdinal("discount_type"))?.ToString(),
                        AllowanceTotal = reader.IsDBNull(reader.GetOrdinal("allowanceTotal")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("allowanceTotal"))),
                        InvoiceBaseTotal = reader.IsDBNull(reader.GetOrdinal("invoiceBaseTotal")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("invoiceBaseTotal"))),
                        InvoiceTaxExclusiveTotal = reader.IsDBNull(reader.GetOrdinal("invoiceTaxExclusiveTotal")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("invoiceTaxExclusiveTotal"))),
                        InvoiceTaxInclusiveTotal = reader.IsDBNull(reader.GetOrdinal("invoiceTaxInclusiveTotal")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("invoiceTaxInclusiveTotal"))),
                        TotalToPay = reader.IsDBNull(reader.GetOrdinal("totalToPay")) ? 0L : Convert.ToInt64(reader.GetValue(reader.GetOrdinal("totalToPay"))),

                        CustomerEntity = new CustomerEntity
                        {
                            IdentificationNumber = reader.GetValue(reader.GetOrdinal("identication"))?.ToString(),
                            TypeOfPerson = reader.IsDBNull(reader.GetOrdinal("person_type")) ? 0 : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("person_type"))),
                            Name = reader.GetValue(reader.GetOrdinal("contact_name"))?.ToString(),
                            Email = reader.GetValue(reader.GetOrdinal("email"))?.ToString(),
                            Phone = reader.GetValue(reader.GetOrdinal("mobile"))?.ToString(),
                            Address = reader.GetValue(reader.GetOrdinal("address_line_1"))?.ToString(),
                            City = reader.GetValue(reader.GetOrdinal("city"))?.ToString(),
                            State = reader.GetValue(reader.GetOrdinal("state"))?.ToString(),
                            Country = reader.GetValue(reader.GetOrdinal("country"))?.ToString(),
                            MunicipalityCode = reader.GetValue(reader.GetOrdinal("custom_field1"))?.ToString(),
                            MunicipalityId = reader.GetValue(reader.GetOrdinal("custom_field2"))?.ToString()
                        },

                        PaymentEntity = new PaymentEntity
                        {
                            PaymentFormId = reader.IsDBNull(reader.GetOrdinal("payment_status")) ? 0 : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("payment_status")))

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
                    d.unit_price,
                    d.tax_id
                FROM v_invoice_detail d
                WHERE d.transaccion IN ({inClause})
                ORDER BY d.transaccion, d.id ASC";

                using var cmdDetails = connection.CreateCommand();
                cmdDetails.CommandText = detailsQuery;
                for (int j = 0; j < chunk.Count; j++)
                {
                    AddParameter(cmdDetails, paramNames[j], chunk[j]);
                }

                using var readerDetails = await cmdDetails.ExecuteReaderAsync(cancellationToken);
                while (await readerDetails.ReadAsync(cancellationToken))
                {
                    int transaccion = readerDetails.GetInt32(readerDetails.GetOrdinal("transaccion"));

                    if (!invoicesMap.TryGetValue(transaccion, out var invoice))
                    {

                        continue;
                    }

                    var item = new ItemElectronicEntity
                    {
                        Transaccion = readerDetails.IsDBNull(readerDetails.GetOrdinal("transaccion")) ? 0 : readerDetails.GetInt32(readerDetails.GetOrdinal("transaccion")),
                        Code = readerDetails.IsDBNull(readerDetails.GetOrdinal("code")) ? 0 : readerDetails.GetInt32(readerDetails.GetOrdinal("code")),
                        TypeItemIdentificationId = readerDetails.IsDBNull(readerDetails.GetOrdinal("type_item_identification_id")) ? 0 : readerDetails.GetInt32(readerDetails.GetOrdinal("type_item_identification_id")),
                        Description = readerDetails.GetValue(readerDetails.GetOrdinal("description"))?.ToString(),
                        UnitMeasureId = readerDetails.IsDBNull(readerDetails.GetOrdinal("unit_measure_id")) ? 0 : readerDetails.GetInt32(readerDetails.GetOrdinal("unit_measure_id")),
                        BaseQuantity = readerDetails.IsDBNull(readerDetails.GetOrdinal("base_quantity")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("base_quantity")),
                        InvoicedQuantity = readerDetails.IsDBNull(readerDetails.GetOrdinal("invoiced_quantity")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("invoiced_quantity")),
                        PriceAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("price_amount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("price_amount")),
                        LineExtensionAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("line_extension_amount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("line_extension_amount")),
                        UnitPriceBeforeDiscount = readerDetails.IsDBNull(readerDetails.GetOrdinal("unit_price_before_discount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("unit_price_before_discount")),
                        LineDiscountAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("line_discount_amount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("line_discount_amount")),
                        LineDiscountType = readerDetails.GetValue(readerDetails.GetOrdinal("line_discount_type"))?.ToString(),
                        Percent = readerDetails.IsDBNull(readerDetails.GetOrdinal("percent")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("percent")),
                        TaxAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("tax_amount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("tax_amount")),
                        UnitPrice = readerDetails.IsDBNull(readerDetails.GetOrdinal("unit_price")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("unit_price")),
                        TaxTotals = !readerDetails.IsDBNull(readerDetails.GetOrdinal("tax_id"))
                            ? new List<TaxTotalEntity>
                            {
                                new TaxTotalEntity
                                {
                                    TaxId = readerDetails.GetInt32(readerDetails.GetOrdinal("tax_id")),
                                    Percent = readerDetails.IsDBNull(readerDetails.GetOrdinal("percent")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("percent")),
                                    TaxAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("tax_amount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("tax_amount")),
                                    TaxableAmount = readerDetails.IsDBNull(readerDetails.GetOrdinal("line_extension_amount")) ? 0.0 : readerDetails.GetDouble(readerDetails.GetOrdinal("line_extension_amount"))
                                }
                            }
                            : new List<TaxTotalEntity>()

                    };

                    invoice.ItemElectronicEntity!.Add(item);
                }
            }

            return invoicesMap.Values.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error connecting to the database for resolution {_dianResolutionEntity.ResolutionId} and server {_server.ServerId} ({_server.Ip}/{_server.DatabaseName})", ex);
        }
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }


}
