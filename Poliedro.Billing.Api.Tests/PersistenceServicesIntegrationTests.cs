using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Common.Pagination;
using Poliedro.Billing.Domain.CompanyProvider.Entities;
using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Resolution.Entities;
using Poliedro.Billing.Domain.Resolution.Enums;
using Poliedro.Billing.Domain.Server.Entities;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Client.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.CompanyProvider.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.DianResolution.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.Server.DomainService.Impl;
using Poliedro.Billing.Infraestructure.Persistence.Mysql.EmissionFailures;

namespace Poliedro.Billing.Api.Tests;

public sealed class PersistenceServicesIntegrationTests
{
    [Fact]
    public async Task EmissionFailureRepository_persists_failure_details_in_isolated_database()
    {
        await using var database = await TestBillingDatabase.CreateAsync();
        var repository = new EmissionFailureRepository(database.Context);
        var failure = new EmissionFailure
        {
            CompanyId = 9,
            ResolutionId = 4,
            ClientName = "Test customer",
            InvoiceNumber = "INV-19",
            Prefix = "TST",
            DocumentType = "FE",
            Reason = "Provider rejected the test invoice",
            FailedAt = new DateTime(2026, 1, 12, 10, 30, 0, DateTimeKind.Utc)
        };

        await repository.RecordAsync(failure, CancellationToken.None);
        var persisted = await database.Context.EmissionFailures.SingleAsync();

        Assert.Equal("Test customer", persisted.ClientName);
        Assert.Equal("INV-19", persisted.InvoiceNumber);
        Assert.Equal("Provider rejected the test invoice", persisted.Reason);
    }

    [Fact]
    public async Task ClientServices_CreateReadUpdateDeleteAndReportMissingRecords()
    {
        await using var database = await TestBillingDatabase.CreateAsync();
        var db = database.Context;
        var token = CancellationToken.None;

        var create = new ClientCreateService(db);
        var exists = new ClientExistsService(db);
        var getAll = new ClientGetAllService(db);
        var getById = new ClientGetByIdService(db, exists);
        var update = new ClientUpdateService(db, exists);
        var delete = new ClientDeleteService(db);

        Assert.True((await create.CreateAsync(Client(1, "Activa", true), token)).IsSuccess);
        Assert.True((await create.CreateAsync(Client(2, "Inactiva", false), token)).IsSuccess);
        Assert.True(await exists.EntityExists(1, token));
        Assert.False(await exists.EntityExists(99, token));
        Assert.Equal("Activa", (await getById.GetByIdAsync(1, token)).Value!.Name);
        Assert.False((await getById.GetByIdAsync(99, token)).IsSuccess);
        Assert.Single((await getAll.GetAllAsync(token)).Value!);

        db.ChangeTracker.Clear(); // Simulate a new request-scoped DbContext for the update.
        Assert.True((await update.UpdateAsync(Client(1, "Renombrada", true), token)).IsSuccess);
        Assert.False((await update.UpdateAsync(Client(99, "Ausente", true), token)).IsSuccess);
        Assert.Equal("Renombrada", (await getById.GetByIdAsync(1, token)).Value!.Name);
        Assert.True((await delete.DeleteAsync(1, token)).IsSuccess);
        Assert.False((await delete.DeleteAsync(99, token)).IsSuccess);
        Assert.False(await exists.EntityExists(1, token));

        var composite = new ClientBillingDomainService(db);
        Assert.True((await composite.CreateAsync(Client(3, "Compuesta", true), token)).IsSuccess);
        Assert.Equal("Compuesta", (await composite.GetByIdAsync(3, token)).Value!.Name);
        Assert.Single((await composite.GetAllAsync(token)).Value!);
        Assert.DoesNotContain((await composite.GetAllAsync(token)).Value!, x => x.CompanyId == 2);
        db.ChangeTracker.Clear(); // A real handler uses a fresh scoped context for each command.
        Assert.True((await composite.UpdateAsync(Client(3, "Actualizada", true), token)).IsSuccess);
        Assert.False((await composite.UpdateAsync(Client(99, "Ausente", true), token)).IsSuccess);
        Assert.True((await composite.DeleteAsync(3, token)).IsSuccess);
        Assert.False((await composite.DeleteAsync(99, token)).IsSuccess);
        Assert.False((await composite.GetByIdAsync(99, token)).IsSuccess);
        Assert.False((await composite.GetAllAsync(token)).IsSuccess);
    }

    [Fact]
    public async Task ServerServices_CreatePaginateReadAndUpdateRecords()
    {
        await using var database = await TestBillingDatabase.CreateAsync();
        var db = database.Context;
        var exists = new ServerExistsService(db);
        var getById = new ServerGetByIdService(db, exists);
        var domain = new ServerDomainService(
            new ServerCreateService(db),
            new ServerUpdateService(db, exists),
            new ServerGetAllService(db),
            getById);

        Assert.True((await domain.CreateAsync(Server(1, "server-1"))).IsSuccess);
        Assert.True((await domain.CreateAsync(Server(2, "server-2"))).IsSuccess);
        Assert.True(await exists.EntityExists(1));
        Assert.False(await exists.EntityExists(99));
        Assert.Equal("server-1", (await domain.GetByIdAsync(1)).Value!.Ip);
        Assert.False((await domain.GetByIdAsync(99)).IsSuccess);
        Assert.Single(await domain.GetAllAsync(new PaginationParams { PageNumber = 2, PageSize = 1 }));

        db.ChangeTracker.Clear(); // Match the request-scoped context used by the API.
        Assert.True((await domain.UpdateAsync(Server(1, "updated-server"))).IsSuccess);
        Assert.False((await domain.UpdateAsync(Server(99, "missing-server"))).IsSuccess);
        Assert.Equal("updated-server", (await domain.GetByIdAsync(1)).Value!.Ip);
    }

    [Fact]
    public async Task DianResolutionServices_HandleActiveFilteringAndCrudLifecycle()
    {
        await using var database = await TestBillingDatabase.CreateAsync();
        var db = database.Context;
        var exists = new DianResolutionExistsService(db);
        var domain = new DianResolutionDomainService(
            new DianResolutionCreateService(db),
            new DianResolutionUpdateService(db, exists),
            new DianResolutionDeleteService(db),
            new DianResolutionGetAllService(db),
            new DianResolutionGetByIdService(db));
        var active = Resolution(1, true);
        var inactive = Resolution(2, false);

        Assert.True((await domain.CreateAsync(active, CancellationToken.None)).IsSuccess);
        Assert.True((await domain.CreateAsync(inactive, CancellationToken.None)).IsSuccess);
        Assert.True(await exists.EntityExists(1, CancellationToken.None));
        Assert.False(await exists.EntityExists(99, CancellationToken.None));
        Assert.Single((await domain.GetAllAsync(CancellationToken.None)).Value!);
        Assert.Equal(1, (await domain.GetByIdAsync(1, CancellationToken.None)).Value!.ResolutionId);

        active.Description = "Actualizada";
        Assert.True((await domain.UpdateAsync(active, CancellationToken.None)).IsSuccess);
        Assert.False((await domain.UpdateAsync(Resolution(99, true), CancellationToken.None)).IsSuccess);
        Assert.Equal("Actualizada", (await domain.GetByIdAsync(1, CancellationToken.None)).Value!.Description);
        Assert.True((await domain.DeleteAsync(active, CancellationToken.None)).IsSuccess);
        Assert.False((await domain.DeleteAsync(active, CancellationToken.None)).IsSuccess);
        Assert.False(await exists.EntityExists(1, CancellationToken.None));
        Assert.False((await domain.GetAllAsync(CancellationToken.None)).IsSuccess);
    }

    [Fact]
    public async Task CompanyProviderServices_HandleCreateLookupUpdateAndDelete()
    {
        await using var database = await TestBillingDatabase.CreateAsync();
        var db = database.Context;
        var create = new CompanyProviderCreateService(db);
        var getById = new CompanyProviderGetByIdSevice(db);
        var update = new CompanyProviderUpdateService(db);
        var delete = new CompanyProviderDeleteService(db);
        var entity = new CompanyProviderEntity
        {
            CompanyProviderId = 1,
            CompanyId = 4,
            ServiceId = 7,
            ProviderId = 1,
            Active = 1,
            ApiKey = "synthetic-test-token"
        };

        Assert.Equal(entity, await create.CreateCompanyProviderAsync(entity, CancellationToken.None));
        Assert.Equal(4, (await getById.GetCompanyProviderByIdAsync(1, CancellationToken.None)).CompanyId);
        Assert.Null(await getById.GetCompanyProviderByIdAsync(99, CancellationToken.None));
        Assert.Null(await update.UpdateCompanyProviderAsync(new CompanyProviderEntity { CompanyProviderId = 99 }, CancellationToken.None));
        entity.Active = 0;
        Assert.Equal(0, (await update.UpdateCompanyProviderAsync(entity, CancellationToken.None))!.Active);
        Assert.False(await delete.DeleteCompanyProviderAsync(99, CancellationToken.None));
        Assert.True(await delete.DeleteCompanyProviderAsync(1, CancellationToken.None));
        Assert.Null(await getById.GetCompanyProviderByIdAsync(1, CancellationToken.None));
    }

    private static ClientEntity Client(int id, string name, bool active) => new()
    {
        CompanyId = id,
        Name = name,
        Nit = $"TEST-{id}",
        Email = $"test{id}@example.invalid",
        Active = active
    };

    private static ServerEntity Server(int id, string host) => new()
    {
        ServerId = id,
        Ip = host,
        DatabaseName = "billing_test",
        DbUsername = "test_user",
        DbPassword = "synthetic-test-password",
        Port = 3306
    };

    private static DianResolutionEntity Resolution(int id, bool active) => new()
    {
        ResolutionId = id,
        CompanyProviderId = 7,
        ResolutionType = ResolutionType.FE,
        Description = $"Resolution {id}",
        ResolutionNumber = $"TEST-{id}",
        Prefix = "TST",
        InitialRange = 1,
        FinalRange = 100,
        CurrentRange = 1,
        ResolutionDate = new DateTime(2026, 1, 1),
        ExpirationDate = new DateTime(2027, 1, 1),
        Active = active
    };
}

internal sealed class TestBillingDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private TestBillingDatabase(SqliteConnection connection, DataBaseContext context)
    {
        _connection = connection;
        Context = context;
    }

    public DataBaseContext Context { get; }

    public static async Task<TestBillingDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<DataBaseContext>()
            .UseSqlite(connection)
            .Options;
        var context = new DataBaseContext(options);
        await context.Database.EnsureCreatedAsync();
        return new TestBillingDatabase(connection, context);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
