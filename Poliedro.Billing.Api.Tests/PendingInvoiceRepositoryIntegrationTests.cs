using Microsoft.Data.Sqlite;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.InvoicesPendingWithDetails.DomainService.Impl;
using System.Data.Common;

namespace Poliedro.Billing.Api.Tests;

public sealed class PendingInvoiceRepositoryIntegrationTests
{
    [Fact]
    public async Task Fe_repository_maps_pending_invoice_customer_payment_and_tax_detail()
    {
        await using var db = await SharedSqliteDatabase.CreateAsync();
        await db.ExecuteAsync("""
            CREATE TABLE invoice_success (verify TEXT);
            CREATE TABLE v_invoice (
                id INTEGER, identication TEXT, person_type INTEGER, contact_name TEXT, email TEXT,
                mobile TEXT, address_line_1 TEXT, city TEXT, state TEXT, country TEXT,
                custom_field1 TEXT, custom_field2 TEXT, invoice TEXT, payment_status INTEGER,
                transaction_date TEXT, total_before_tax NUMERIC, discount_amount_by_invoice NUMERIC,
                discount_type TEXT, allowanceTotal NUMERIC, invoiceBaseTotal NUMERIC,
                invoiceTaxExclusiveTotal NUMERIC, invoiceTaxInclusiveTotal NUMERIC,
                totalToPay NUMERIC, send_dian INTEGER);
            CREATE TABLE v_invoice_detail (
                transaccion INTEGER, code INTEGER, type_item_identification_id INTEGER,
                description TEXT, unit_measure_id INTEGER, base_quantity REAL, invoiced_quantity REAL,
                price_amount REAL, line_extension_amount REAL, unit_price_before_discount REAL,
                line_discount_amount REAL, line_discount_type TEXT, percent REAL, tax_amount REAL,
                unit_price REAL, tax_id INTEGER, id INTEGER);
            INSERT INTO v_invoice VALUES
              (41, '900111222', 1, 'Cliente de prueba', 'cliente@example.invalid', '3000000000',
               'Calle 1', 'Bogotá', 'Cundinamarca', 'CO', '11001', '1', 'FE-41', 10,
               '2026-10-22 09:30:00', 100, 0, NULL, 0, 100, 100, 119, 119, 1);
            INSERT INTO v_invoice_detail VALUES
              (41, 7, 1, 'Producto', 70, 1, 1, 100, 100, 100, 0, NULL, 19, 19, 100, 1, 1);
            """);

        var repository = new InvoicesPendingWithDetailsFERepository(db.Factory);
        var results = (await repository.GetAllInvoicePendingWithDetails(Server(), Resolution(ResolutionType.FE), CancellationToken.None)).ToList();

        var invoice = Assert.Single(results);
        Assert.Equal("FE-41", invoice.Number);
        Assert.Equal(119, invoice.TotalToPay);
        Assert.Equal("Cliente de prueba", invoice.CustomerEntity!.Name);
        Assert.Equal("900111222", invoice.CustomerEntity.IdentificationNumber);
        Assert.Equal(10, invoice.PaymentEntity!.PaymentFormId);
        var item = Assert.Single(invoice.ItemElectronicEntity!);
        Assert.Equal("Producto", item.Description);
        Assert.Equal(1, Assert.Single(item.TaxTotals!).TaxId);
        Assert.Equal(19, item.TaxTotals![0].TaxAmount);
    }

    [Fact]
    public async Task Fe_repository_filters_out_paid_dian_disabled_and_zero_total_invoices()
    {
        await using var db = await SharedSqliteDatabase.CreateAsync();
        await db.ExecuteAsync("""
            CREATE TABLE invoice_success (verify TEXT);
            CREATE TABLE v_invoice (
                id INTEGER, identication TEXT, person_type INTEGER, contact_name TEXT, email TEXT,
                mobile TEXT, address_line_1 TEXT, city TEXT, state TEXT, country TEXT,
                custom_field1 TEXT, custom_field2 TEXT, invoice TEXT, payment_status INTEGER,
                transaction_date TEXT, total_before_tax NUMERIC, discount_amount_by_invoice NUMERIC,
                discount_type TEXT, allowanceTotal NUMERIC, invoiceBaseTotal NUMERIC,
                invoiceTaxExclusiveTotal NUMERIC, invoiceTaxInclusiveTotal NUMERIC,
                totalToPay NUMERIC, send_dian INTEGER);
            INSERT INTO v_invoice VALUES
              (1, '1', 1, 'Emitida', '', '', '', '', '', '', '', '', 'PAID', 1, '2026-10-22', 1, 0, NULL, 0, 1, 1, 1, 1, 1),
              (2, '2', 1, 'No enviar', '', '', '', '', '', '', '', '', 'NO-DIAN', 1, '2026-10-22', 1, 0, NULL, 0, 1, 1, 1, 1, 0),
              (3, '3', 1, 'Cero', '', '', '', '', '', '', '', '', 'ZERO', 1, '2026-10-22', 0, 0, NULL, 0, 0, 0, 0, 0, 1);
            INSERT INTO invoice_success VALUES ('PAID');
            """);

        var repository = new InvoicesPendingWithDetailsFERepository(db.Factory);
        var automaticNo = Resolution(ResolutionType.FE);
        automaticNo.Automatic = (int)Poliedro.Billing.Domain.Common.Enum.Automatic.No;
        var results = await repository.GetAllInvoicePendingWithDetails(Server(), automaticNo, CancellationToken.None);

        Assert.Empty(results);
    }

    [Fact]
    public async Task Pos_repository_groups_invoice_details_and_ignores_rows_without_invoice_number()
    {
        await using var db = await SharedSqliteDatabase.CreateAsync();
        await db.ExecuteAsync("""
            CREATE TABLE invoice_success (verify TEXT);
            CREATE TABLE v_api_invoice (
                id INTEGER, Date TEXT, Time TEXT, Resolution TEXT, prefix TEXT, number TEXT,
                resolutionType TEXT, note TEXT, allowanceTotal NUMERIC, invoiceBaseTotal NUMERIC,
                invoiceTaxExclusiveTotal NUMERIC, invoiceTaxInclusiveTotal NUMERIC, totalToPay NUMERIC);
            CREATE TABLE v_api_item (
                resolution TEXT, description TEXT, code TEXT, base_quantity REAL,
                invoiced_quantity REAL, price_amount REAL, subtotal REAL);
            INSERT INTO v_api_invoice VALUES
              (51, '2026-10-22 10:00:00', '10:00:00', 'R-51', 'POS', '51', 'POS', 'nota', 0, 50, 50, 60, 60),
              (52, '2026-10-22 10:01:00', '10:01:00', 'R-52', 'POS', NULL, 'POS', NULL, 0, 0, 0, 0, 0);
            INSERT INTO v_api_item VALUES ('R-51', 'Item A', '7', 1, 1, 50, 50);
            INSERT INTO v_api_item VALUES ('R-51', 'Item B', 'not-numeric', 2, 2, 10, 20);
            INSERT INTO v_api_item VALUES ('R-52', 'Item ignored', '8', 1, 1, 1, 1);
            """);

        var repository = new InvoicesPendingWithDetailsPOSRepository(db.Factory);
        var results = (await repository.GetAllInvoicePendingWithDetails(Server(), Resolution(ResolutionType.POS), CancellationToken.None)).ToList();

        var invoice = Assert.Single(results);
        Assert.Equal("51", invoice.Number);
        Assert.Equal("POS", invoice.Prefix);
        Assert.Equal("R-51", invoice.Resolution);
        Assert.Equal(60, invoice.TotalToPay);
        Assert.Collection(invoice.ItemElectronicEntity!,
            item => { Assert.Equal("Item A", item.Description); Assert.Equal(7, item.Code); },
            item => { Assert.Equal("Item B", item.Description); Assert.Null(item.Code); });
    }

    [Fact]
    public async Task Pos_repository_returns_empty_when_there_are_no_pending_rows()
    {
        await using var db = await SharedSqliteDatabase.CreateAsync();
        await db.ExecuteAsync("""
            CREATE TABLE invoice_success (verify TEXT);
            CREATE TABLE v_api_invoice (
                id INTEGER, Date TEXT, Time TEXT, Resolution TEXT, prefix TEXT, number TEXT,
                resolutionType TEXT, note TEXT, allowanceTotal NUMERIC, invoiceBaseTotal NUMERIC,
                invoiceTaxExclusiveTotal NUMERIC, invoiceTaxInclusiveTotal NUMERIC, totalToPay NUMERIC);
            CREATE TABLE v_api_item (
                resolution TEXT, description TEXT, code TEXT, base_quantity REAL,
                invoiced_quantity REAL, price_amount REAL, subtotal REAL);
            """);

        var repository = new InvoicesPendingWithDetailsPOSRepository(db.Factory);
        var results = await repository.GetAllInvoicePendingWithDetails(Server(), Resolution(ResolutionType.POS), CancellationToken.None);

        Assert.Empty(results);
    }

    private static ServerEntity Server() => new() { ServerId = 1, Ip = "sqlite-memory", DatabaseName = "isolated" };

    private static DianResolutionEntity Resolution(ResolutionType type) => new()
    {
        ResolutionId = 3,
        ResolutionType = type,
        ResolutionDate = new DateTime(2026, 10, 1),
        Automatic = (int)Poliedro.Billing.Domain.Common.Enum.Automatic.Yes
    };

    private sealed class SqliteServerConnectionFactory(string connectionString) : IServerDatabaseConnectionFactory
    {
        public DbConnection Create(ServerEntity server) => new SqliteConnection(connectionString);
    }

    private sealed class SharedSqliteDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _anchor;
        public IServerDatabaseConnectionFactory Factory { get; }

        private SharedSqliteDatabase(SqliteConnection anchor, string connectionString)
        {
            _anchor = anchor;
            Factory = new SqliteServerConnectionFactory(connectionString);
        }

        public static async Task<SharedSqliteDatabase> CreateAsync()
        {
            var connectionString = $"Data Source=billing-tests-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
            var anchor = new SqliteConnection(connectionString);
            await anchor.OpenAsync();
            return new SharedSqliteDatabase(anchor, connectionString);
        }

        public async Task ExecuteAsync(string sql)
        {
            await using var command = _anchor.CreateCommand();
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }

        public ValueTask DisposeAsync() => _anchor.DisposeAsync();
    }
}
