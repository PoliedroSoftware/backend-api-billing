using System.Data.Common;
using MySqlConnector;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.Persistence.Mysql.Context;

public sealed class MySqlServerDatabaseConnectionFactory(IDatabaseUtils databaseUtils)
    : IServerDatabaseConnectionFactory, IRelationalConnectionFactory
{
    public DbConnection Create(ServerEntity server)
        => Create(databaseUtils.GetConnectionString(server));

    public DbConnection Create(string connectionString)
        => new MySqlConnection(connectionString);
}
