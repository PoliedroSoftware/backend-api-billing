using Microsoft.Data.Sqlite;
using Poliedro.Billing.Domain.FERetail.Ports;
using System.Data.Common;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiItemQueryIntegrationTests
{
    [Fact]
    public async Task Fe_item_query_maps_positive_lines_and_skips_non_billable_amounts()
    {
        await using var db = await ItemDatabase.CreateAsync();
        await db.ExecuteAsync("""
            CREATE TABLE v_invoice_detail (
                id INTEGER, transaccion INTEGER, code INTEGER, type_item_identification_id INTEGER,
                description TEXT, unit_measure_id INTEGER, base_quantity REAL, invoiced_quantity REAL,
                price_amount REAL, line_extension_amount REAL, line_discount_amount REAL,
                line_discount_type TEXT, percent REAL, tax_amount REAL, unit_price REAL);
            INSERT INTO v_invoice_detail VALUES
              (1, 20, 101, 1, 'Combustible', 70, 2, 2, 5000, 10000, 0, NULL, 19, 1900, 5000),
              (2, 20, 102, 1, 'Sin valor', 70, 1, 1, 0, 0, 0, NULL, 0, 0, 0),
              (3, 21, 103, 1, 'Otra factura', 70, 1, 1, 2000, 2000, 0, NULL, 19, 380, 2000);
            """);
        var service = new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.GetItem(db.Factory);

        var items = await service.GetItemsAsync("20", "unused-by-test-factory");

        var item = Assert.Single(items);
        Assert.Equal(1, item.id);
        Assert.Equal(20, item.transaccion);
        Assert.Equal(101, item.code);
        Assert.Equal("Combustible", item.description);
        Assert.Equal(10000, item.line_extension_amount);
        Assert.Equal(1900, item.tax_amount);
    }

    [Fact]
    public async Task Pos_item_query_selects_invoice_by_suffix_and_maps_its_lines()
    {
        await using var db = await ItemDatabase.CreateAsync();
        await db.ExecuteAsync("""
            CREATE TABLE v_api_invoice (resolution TEXT, number TEXT);
            CREATE TABLE v_api_item (
                resolution TEXT, description TEXT, code TEXT, base_quantity REAL,
                invoiced_quantity REAL, price_amount NUMERIC, subtotal REAL);
            INSERT INTO v_api_invoice VALUES ('R-100', '123'), ('R-200', '124');
            INSERT INTO v_api_item VALUES
              ('R-100', 'Venta POS', 'SKU-7', 1, 1, 12500, 12500),
              ('R-100', 'Renglón gratuito', 'SKU-8', 1, 1, 0, 0),
              ('R-200', 'Otra venta', 'SKU-9', 1, 1, 3000, 3000);
            """);
        var service = new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS.GetItem(db.Factory);

        var items = await service.GetItemsAsync(123, "unused-by-test-factory");

        var item = Assert.Single(items);
        Assert.Equal("R-100", item.Resolution);
        Assert.Equal("Venta POS", item.Description);
        Assert.Equal("SKU-7", item.Code);
        Assert.Equal(12500, item.PriceAmount);
        Assert.Equal(12500, item.Subtotal);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Item_query_returns_empty_when_no_table_or_matching_rows_exist(bool fe)
    {
        await using var db = await ItemDatabase.CreateAsync();
        if (fe)
            await db.ExecuteAsync("CREATE TABLE v_invoice_detail (id INTEGER, transaccion INTEGER, code INTEGER, type_item_identification_id INTEGER, description TEXT, unit_measure_id INTEGER, base_quantity REAL, invoiced_quantity REAL, price_amount REAL, line_extension_amount REAL, line_discount_amount REAL, line_discount_type TEXT, percent REAL, tax_amount REAL, unit_price REAL);");
        else
            await db.ExecuteAsync("CREATE TABLE v_api_invoice (resolution TEXT, number TEXT); CREATE TABLE v_api_item (resolution TEXT, description TEXT, code TEXT, base_quantity REAL, invoiced_quantity REAL, price_amount NUMERIC, subtotal REAL);");

        if (fe)
        {
            var actual = await new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.GetItem(db.Factory)
                .GetItemsAsync("5", "unused");
            Assert.Empty(actual);
        }
        else
        {
            var actual = await new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS.GetItem(db.Factory)
                .GetItemsAsync(5, "unused");
            Assert.Empty(actual);
        }
    }

    private sealed class ItemDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _anchor;
        public IRelationalConnectionFactory Factory { get; }

        private ItemDatabase(SqliteConnection anchor, string connectionString)
        {
            _anchor = anchor;
            Factory = new SqliteFactory(connectionString);
        }

        public static async Task<ItemDatabase> CreateAsync()
        {
            var connectionString = $"Data Source=plemsi-items-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
            var anchor = new SqliteConnection(connectionString);
            await anchor.OpenAsync();
            return new ItemDatabase(anchor, connectionString);
        }

        public async Task ExecuteAsync(string sql)
        {
            await using var command = _anchor.CreateCommand();
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }

        public ValueTask DisposeAsync() => _anchor.DisposeAsync();
    }

    private sealed class SqliteFactory(string connectionString) : IRelationalConnectionFactory
    {
        public DbConnection Create(string _)
        {
            var connection = new SqliteConnection(connectionString);
            connection.CreateFunction<string?, long, long>("RIGHT", (value, length) =>
                value is not null && long.TryParse(value[^Math.Min(value.Length, (int)length)..], out var suffix)
                    ? suffix : 0);
            return connection;
        }
    }
}
