using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.Client.Enums;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Impl;

public class DianResolutionCreditNoteDomainService : IBillingGetInfgoClientCreditNote
{
    public Task<BillingInfoClient> BillingInfoClientCreditNote(ClientEntity clientEntity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
