using MySqlConnector;
using Poliedro.Billing.Domain.BillingPos.Ports;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.POS.EDS;

public class DatabaseUtils : IDatabaseUtilsPos
{
    public string GetConnectionString(ServerEntity server)
    {
        return new MySqlConnectionStringBuilder
        {
            Server = server.Ip,
            Database = server.DatabaseName,
            UserID = server.DbUsername,
            Password = server.DbPassword,
            SslMode = MySqlSslMode.None
        }.ConnectionString;
    }

}
