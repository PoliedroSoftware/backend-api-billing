using Poliedro.Billing.Domain.Server.Entities;

namespace Poliedro.Billing.Domain.BillingPos.Ports
{
    public interface IDatabaseUtilsPos
    {
        string GetConnectionString(ServerEntity server);
    }

}
