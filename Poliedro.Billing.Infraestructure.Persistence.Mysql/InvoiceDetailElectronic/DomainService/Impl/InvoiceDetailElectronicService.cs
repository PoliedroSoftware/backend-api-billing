using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Entities;
using Poliedro.Billing.Domain.InvoiceDetailElectronic.Ports;
using Poliedro.Billing.Domain.Server.Entities;
using System.Data;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoiceDetailElectronic.DomainService.Impl
{
    public class InvoiceDetailElectronicService() : IInvoiceElectronicRepository
    {
        public async Task<(IEnumerable<InvoiceElectronic> Data, int TotalCount)> GetAllInvoiceElectronicWithDetails(
    ServerEntity server,
    IDatabaseUtils databaseUtils,
    InvoiceElectronicParameters parameters,
    CancellationToken cancellationToken)
        {
            var invoicesWithDetails = new List<InvoiceElectronic>();
            using MySqlConnection connection = new(databaseUtils.GetConnectionString(server));

            string orderDirection = parameters.OrderBy?.ToLower() == "desc" ? "DESC" : "ASC";
            int skip = (parameters.PageNumber - 1) * parameters.PageSize;

            try
            {
                await connection.OpenAsync(cancellationToken);
                var countQuery = "SELECT COUNT(DISTINCT id) FROM v_invoice_electronic";
                using var countCommand = new MySqlCommand(countQuery, connection);
                var totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));

                var query = $@"
            SELECT
                ie.id AS invoice_id,
                ie.identication,
                ie.contact_name,
                ie.email,
                ie.mobile,
                ie.city,
                ie.state,
                ie.country,
                ie.invoice AS invoice_number,
                ie.payment_status,
                ie.transaction_date,
                ie.allowanceTotal,
                ie.invoiceBaseTotal,
                ie.invoiceTaxExclusiveTotal,
                ie.invoiceTaxInclusiveTotal,
                ie.totalToPay,
                ie.created_by_name,

                ied.id AS detail_id,
                ied.transaccion,
                ied.code,
                ied.type_item_identification_id,
                ied.description,
                ied.unit_measure_id,
                ied.base_quantity,
                ied.invoiced_quantity,
                ied.price_amount,
                ied.line_extension_amount,
                ied.percent,
                ied.tax_amount,
                ied.unit_price,
                ied.invoice AS detail_invoice,
                ied.dateregister,
                ied.verify
            FROM
                v_invoice_electronic ie
            LEFT JOIN
                v_invoice_detail_electronic ied ON ie.id = ied.transaccion
            ORDER BY
                ie.id {orderDirection}
            LIMIT @Skip, @Take;";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", parameters.PageSize);

                using var reader = await command.ExecuteReaderAsync(cancellationToken);

                int? currentInvoiceId = null;
                InvoiceElectronic? invoiceElectronic = null;

                while (await reader.ReadAsync(cancellationToken))
                {
                    int invoiceId = reader.GetInt32("invoice_id");

                    if (currentInvoiceId != invoiceId)
                    {
                        currentInvoiceId = invoiceId;

                        invoiceElectronic = new InvoiceElectronic
                        {
                            Id = invoiceId,
                            Identication = reader.IsDBNull("identication") ? null : reader.GetString("identication"),
                            ContactName = reader.IsDBNull("contact_name") ? null : reader.GetString("contact_name"),
                            Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                            Mobile = reader.IsDBNull("mobile") ? null : reader.GetString("mobile"),
                            City = reader.IsDBNull("city") ? null : reader.GetString("city"),
                            State = reader.IsDBNull("state") ? null : reader.GetString("state"),
                            Country = reader.IsDBNull("country") ? null : reader.GetString("country"),
                            Invoice = reader.IsDBNull("invoice_number") ? null : reader.GetString("invoice_number"),
                            PaymentStatus = reader.IsDBNull("payment_status") ? null : reader.GetString("payment_status"),
                            TransactionDate = reader.IsDBNull("transaction_date") ? null : reader.GetDateTime("transaction_date"),
                            AllowanceTotal = reader.IsDBNull("allowanceTotal") ? 0 : reader.GetInt32("allowanceTotal"),
                            InvoiceBaseTotal = reader.IsDBNull("invoiceBaseTotal") ? 0 : reader.GetDecimal("invoiceBaseTotal"),
                            InvoiceTaxExclusiveTotal = reader.IsDBNull("invoiceTaxExclusiveTotal") ? 0 : reader.GetDecimal("invoiceTaxExclusiveTotal"),
                            InvoiceTaxInclusiveTotal = reader.IsDBNull("invoiceTaxInclusiveTotal") ? 0 : reader.GetDecimal("invoiceTaxInclusiveTotal"),
                            TotalToPay = reader.IsDBNull("totalToPay") ? 0 : reader.GetDecimal("totalToPay"),
                            CreatedByName = reader.IsDBNull("created_by_name") ? null : reader.GetString("created_by_name"),
                            InvoiceElectronicDetails = new List<InvoiceWithDetailElectronic>()
                        };

                        invoicesWithDetails.Add(invoiceElectronic);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("detail_id")))
                    {
                        var detail = new InvoiceWithDetailElectronic
                        {
                            Id = reader.GetInt32("detail_id"),
                            Transaccion = reader.IsDBNull("transaccion") ? 0 : reader.GetInt32("transaccion"),
                            Code = reader.IsDBNull("code") ? 0 : reader.GetInt32("code"),
                            TypeItemIdentification = reader.IsDBNull("type_item_identification_id") ? 0 : reader.GetInt32("type_item_identification_id"),
                            Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                            UnitMeasureId = reader.IsDBNull("unit_measure_id") ? 0 : reader.GetInt32("unit_measure_id"),
                            BaseQuantity = reader.IsDBNull("base_quantity") ? 0 : reader.GetDecimal("base_quantity"),
                            InvoicedQuantity = reader.IsDBNull("invoiced_quantity") ? 0 : reader.GetDecimal("invoiced_quantity"),
                            PriceAmount = reader.IsDBNull("price_amount") ? null : reader.GetDecimal("price_amount"),
                            LineExtensionAmount = reader.IsDBNull("line_extension_amount") ? null : reader.GetDecimal("line_extension_amount"),
                            Percent = reader.IsDBNull("percent") ? null : reader.GetDouble("percent"),
                            TaxAmount = reader.IsDBNull("tax_amount") ? 0 : reader.GetDecimal("tax_amount"),
                            UnitPrice = reader.IsDBNull("unit_price") ? null : reader.GetDecimal("unit_price"),
                            Invoice = reader.IsDBNull("detail_invoice") ? null : reader.GetString("detail_invoice"),
                            DateRegister = reader.IsDBNull("dateregister") ? null : reader.GetDateTime("dateregister"),
                            Verify = reader.IsDBNull("verify") ? null : reader.GetString("verify")
                        };

                        invoiceElectronic?.InvoiceElectronicDetails.Add(detail);
                    }
                }

                return (invoicesWithDetails, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching invoice electronic details", ex);
            }
        }

    }
}
