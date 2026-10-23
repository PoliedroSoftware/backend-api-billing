using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Moq;
using Poliedro.Billing.Domain.Common.Results;
using Poliedro.Billing.Domain.Common.Results.Errors;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Resolution.DomainService;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Domain.UpdateCurrentlyNumber.Port;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.UpdateCurrentlyNumber;
using System.Data.Common;
using System.Net;

namespace Poliedro.Billing.Api.Tests;

public sealed class UpdateCurrentlyNumberServiceTests
{
    [Fact]
    public async Task Updates_consecutive_and_expiration_flag_in_isolated_database()
    {
        await using var database = await LocalDatabase.CreateAsync();
        await database.ExecuteAsync("CREATE TABLE dian_resolution_billing (resolution_id INTEGER PRIMARY KEY, current_number INTEGER, date_current TEXT, expirated INTEGER); INSERT INTO dian_resolution_billing VALUES (4, 10, NULL, 0);");
        var factory = new SqliteConnectionFactory(database.ConnectionString);
        var service = CreateService(factory, currentNumber: 10);

        await service.UpdateCurrentlyNumberAsync(new ParametersCurrentlyNumber(11, "2026-10-22 12:30:00", 4, null, Expirated: true), CancellationToken.None);

        var row = await database.ReadRowAsync();
        Assert.Equal(11, row.CurrentNumber);
        Assert.Equal("2026-10-22 12:30:00", row.DateCurrent);
        Assert.Equal(1, row.Expirated);
        Assert.Equal(2, factory.CreatedConnections);
    }

    [Fact]
    public async Task Does_not_rewind_current_consecutive_or_open_a_database_when_resolution_is_missing()
    {
        await using var database = await LocalDatabase.CreateAsync();
        await database.ExecuteAsync("CREATE TABLE dian_resolution_billing (resolution_id INTEGER PRIMARY KEY, current_number INTEGER, date_current TEXT, expirated INTEGER); INSERT INTO dian_resolution_billing VALUES (4, 10, NULL, 0);");
        var factory = new SqliteConnectionFactory(database.ConnectionString);
        var service = CreateService(factory, currentNumber: 10);

        await service.UpdateCurrentlyNumberAsync(new ParametersCurrentlyNumber(9, null, 4, null), CancellationToken.None);

        Assert.Equal(0, factory.CreatedConnections);
        Assert.Equal(10, (await database.ReadRowAsync()).CurrentNumber);

        var missingService = CreateService(factory, resolutionFound: false);
        await missingService.UpdateCurrentlyNumberAsync(new ParametersCurrentlyNumber(99, null, 404, null), CancellationToken.None);
        Assert.Equal(0, factory.CreatedConnections);
    }

    private static UpdateCurrentlyNumberService CreateService(SqliteConnectionFactory factory, int currentNumber = 0, bool resolutionFound = true)
    {
        var resolutions = new Mock<IDianResolutionDomainService>();
        resolutions.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resolutionFound
                ? Result<DianResolutionEntity, Error>.Success(new DianResolutionEntity { ResolutionId = 4, CurrentRange = currentNumber })
                : Result<DianResolutionEntity, Error>.Failure(Error.CreateInstance("missing", "missing", HttpStatusCode.NotFound)));
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:MysqlConnection"] = "test-only"
        }).Build();
        return new UpdateCurrentlyNumberService(resolutions.Object, configuration, factory);
    }

    private sealed class SqliteConnectionFactory(string connectionString) : IRelationalConnectionFactory
    {
        public int CreatedConnections { get; private set; }
        public DbConnection Create(string _) { CreatedConnections++; return new SqliteConnection(connectionString); }
    }

    private sealed class LocalDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _anchor;
        public string ConnectionString { get; }
        private LocalDatabase(SqliteConnection anchor, string connectionString) { _anchor = anchor; ConnectionString = connectionString; }
        public static async Task<LocalDatabase> CreateAsync()
        {
            var cs = $"Data Source=consecutive-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
            var anchor = new SqliteConnection(cs);
            await anchor.OpenAsync();
            return new LocalDatabase(anchor, cs);
        }
        public async Task ExecuteAsync(string sql)
        {
            await using var command = _anchor.CreateCommand(); command.CommandText = sql; await command.ExecuteNonQueryAsync();
        }
        public async Task<(int CurrentNumber, string? DateCurrent, int Expirated)> ReadRowAsync()
        {
            await using var command = _anchor.CreateCommand(); command.CommandText = "SELECT current_number, date_current, expirated FROM dian_resolution_billing WHERE resolution_id = 4";
            await using var reader = await command.ExecuteReaderAsync(); await reader.ReadAsync();
            return (reader.GetInt32(0), reader.IsDBNull(1) ? null : reader.GetString(1), reader.GetInt32(2));
        }
        public ValueTask DisposeAsync() => _anchor.DisposeAsync();
    }
}
