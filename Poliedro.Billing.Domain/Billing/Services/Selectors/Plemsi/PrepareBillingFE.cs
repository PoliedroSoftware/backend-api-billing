using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Domain.Billing.Services.Selectors.Plemsi;

public class PrepareBillingFE : ICreateBillingStrategy
{
    public Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken)
    {

        return Task.FromResult(invoices);
    }
}