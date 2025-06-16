using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface IDatabaseUtils
{
    string GetConnectionString(ServerEntity server);
}
