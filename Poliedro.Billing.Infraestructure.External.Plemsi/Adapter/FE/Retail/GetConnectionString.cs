using MySqlConnector;
using Poliedro.Billing.Domain.FERetail.Ports;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.FE.Retail;

public class DatabaseUtils : IDatabaseUtils
{
    public string GetConnectionString(ServerEntity server)
    {
        return new MySqlConnectionStringBuilder
        {
            Server = server.Ip,
            Port = server.Port,
            Database = server.DatabaseName,
            UserID = server.DbUsername,
            Password = server.DbPassword,
            SslMode = MySqlSslMode.None
        }.ConnectionString;
    }

}