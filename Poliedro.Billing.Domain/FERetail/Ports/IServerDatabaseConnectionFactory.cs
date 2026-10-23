using System.Data.Common;
using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.FERetail.Ports;

/// <summary>Creates a connection to the billing database associated with a server.</summary>
public interface IServerDatabaseConnectionFactory
{
    DbConnection Create(ServerEntity server);
}
