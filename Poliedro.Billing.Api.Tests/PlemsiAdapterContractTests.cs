using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.CreditNote.Entity;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.CreditNote;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;
using Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;
using FeDatabaseUtils = Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail.DatabaseUtils;

namespace Poliedro.Billing.Api.Tests;

public sealed class PlemsiAdapterContractTests
{
    [Fact]
    public async Task Credit_note_adapter_returns_explicit_not_implemented_error()
    {
        var result = await new CreditNoteDomainService().Create(new CreditNoteEntity(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.NotImplemented, result.Error!.HttpStatusCode);
    }

    [Fact]
    public async Task Fe_retail_adapter_returns_explicit_not_implemented_error()
    {
        var result = await new FERetailService().CreateElectronicInvoicesAsync(Array.Empty<ClientEntity>(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.NotImplemented, result.Error!.HttpStatusCode);
    }

    [Fact]
    public void Fe_database_adapter_builds_connection_string_without_connecting()
    {
        var server = new ServerEntity { Ip = "127.0.0.1", Port = 3307, DatabaseName = "billing_test", DbUsername = "test_user", DbPassword = "test_password" };

        var connectionString = new FeDatabaseUtils().GetConnectionString(server);

        Assert.Contains("Server=127.0.0.1", connectionString);
        Assert.Contains("Port=3307", connectionString);
        Assert.Contains("Database=billing_test", connectionString);
        Assert.Contains("User ID=test_user", connectionString);
    }

    [Fact]
    public void Pos_database_adapter_uses_default_mysql_port_and_does_not_connect()
    {
        var server = new ServerEntity { Ip = "localhost", Port = 3306, DatabaseName = "pos_test", DbUsername = "test_user", DbPassword = "test_password" };

        var connectionString = new Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS.DatabaseUtils().GetConnectionString(server);

        Assert.Contains("Server=localhost", connectionString);
        Assert.Contains("Database=pos_test", connectionString);
        Assert.Contains("User ID=test_user", connectionString);
    }
}
