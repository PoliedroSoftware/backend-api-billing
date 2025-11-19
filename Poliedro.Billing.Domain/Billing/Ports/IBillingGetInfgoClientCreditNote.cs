using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingGetInfgoClientCreditNote
{
    Task<BillingInfoClient> BillingInfoClientCreditNote(ClientEntity clientEntity, CancellationToken cancellationToken);
}
