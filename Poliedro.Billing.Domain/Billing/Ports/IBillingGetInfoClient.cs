using Poliedro.Billing.Domain.Client.Entities;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingGetInfoClient
{
    Task<BillingInfoClient> BillingInfoClient(ClientEntity clientEntity, CancellationToken cancellationToken);
}
