using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBilling //POS
{
    public Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, ClientEntity clientEntity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}