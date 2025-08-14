using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports
{
    public interface IDatabaseUtilsPos
    {
        string GetConnectionString(ServerEntity server);
    }

}
