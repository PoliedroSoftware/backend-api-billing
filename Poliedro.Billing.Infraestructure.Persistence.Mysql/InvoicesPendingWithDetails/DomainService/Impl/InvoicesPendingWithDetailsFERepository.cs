using MySqlConnector;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Enum;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoicesPendingWithDetails.Ports;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;

public class InvoicesPendingWithDetailsFERepository : IInvoicesPendingWithDetailsStrategy
{
    public async Task<IEnumerable<CreateBilling>> GetAllInvoicePendingWithDetails(
    ServerEntity server,
    ClientEntity clientItem,
    IDatabaseUtils databaseUtils,
    CancellationToken cancellationToken)
    {
        var invoicesMap = new Dictionary<int, CreateBilling>();
        using MySqlConnection connection = new(databaseUtils.GetConnectionString(server));

        try
        {
            await connection.OpenAsync(cancellationToken);

            string query = @"
                SELECT 
                    v.id AS invoice_id,
                    v.identication,
                    v.contact_name,
                    v.email,
                    v.mobile,
                    v.city,
                    v.state,
                    v.country,
                    v.invoice,
                    v.payment_status,
                    v.transaction_date,
                    v.allowanceTotal,
                    v.invoiceBaseTotal,
                    v.invoiceTaxExclusiveTotal,
                    v.invoiceTaxInclusiveTotal,
                    v.totalToPay,
                    v.send_dian,

                    d.id AS detail_id,
                    d.transaccion,
                    d.code,
                    d.type_item_identification_id,
                    d.description,
                    d.unit_measure_id,
                    d.base_quantity,
                    d.invoiced_quantity,
                    d.price_amount,
                    d.line_extension_amount,
                    d.percent,
                    d.tax_amount,
                    d.unit_price
                FROM v_invoice v
                LEFT JOIN invoice_success i ON v.invoice = i.verify
                INNER JOIN v_invoice_detail d ON v.invoice = d.transaccion
                WHERE i.verify IS NULL
                AND v.transaction_date >= @date "
                + ((Automatic)clientItem.Automatic == Automatic.No ? " AND v.send_dian = 1 " : "")
                + " ORDER BY v.id ASC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", clientItem.Date.ToString("yyyy-MM-dd"));
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                int invoiceId = reader.GetInt32("invoice_id");

                if (!invoicesMap.TryGetValue(invoiceId, out var invoice))
                {
                    bool addInvoice = ((Automatic)clientItem.Automatic == Automatic.No &&
                                        Convert.ToInt32(reader["send_dian"]) == (int)Automatic.Yes)
                                      || ((Automatic)clientItem.Automatic == Automatic.Yes &&
                                          Convert.ToInt64(reader["totalToPay"]) > 0);

                    if (!addInvoice)
                        continue;

                    invoice = new CreateBilling
                    {
                        Number = reader["invoice"].ToString(),
                        //Payment_status = reader["payment_status"].ToString(),
                        TransactionDate = reader["transaction_date"].Equals(DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(reader["transaction_date"]),
                        AllowanceTotal = Convert.ToInt64(reader["allowanceTotal"]),
                        InvoiceBaseTotal = Convert.ToInt64(reader["invoiceBaseTotal"]),
                        InvoiceTaxExclusiveTotal = Convert.ToInt64(reader["invoiceTaxExclusiveTotal"]),
                        InvoiceTaxInclusiveTotal = Convert.ToInt64(reader["invoiceTaxInclusiveTotal"]),
                        TotalToPay = Convert.ToInt64(reader["totalToPay"]),


                        CustomerEntity = new CustomerEntity
                        {
                            IdentificationNumber = reader["identication"].ToString(),
                            Name = reader["contact_name"].ToString(),
                            Email = reader["email"].ToString(),
                            Phone = reader["mobile"].ToString(),
                            City = reader["city"].ToString(),
                            State = reader["state"].ToString(),
                            Country = reader["country"].ToString()
                        },

                        ItemElectronicEntity = new List<ItemElectronicEntity>()

                    };

                    invoicesMap[invoiceId] = invoice;
                }

               var item = new ItemElectronicEntity
               {
                    Transaccion = reader.GetInt32("transaccion"),
                    Code = reader.GetInt32("code"),
                    TypeItemIdentificationId = reader.GetInt32("type_item_identification_id"),
                    Description = reader["description"].ToString(),
                    UnitMeasureId = reader.GetInt32("unit_measure_id"),
                    BaseQuantity = reader.GetDouble("base_quantity"),
                    InvoicedQuantity = reader.GetDouble("invoiced_quantity"),
                    PriceAmount = reader.GetDouble("price_amount"),
                    LineExtensionAmount = reader.GetDouble("line_extension_amount"),
                    Percent = reader.GetDouble("percent"),
                    TaxAmount = reader.GetDouble("tax_amount"),
                    UnitPrice = reader.GetDouble("unit_price")
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
