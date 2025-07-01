using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.BillingPos.Ports;

namespace Poliedro.Billing.Application.BillingPos.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBillingStrategy
{
    public Task<List<CreateBilling>> CreateInvoicesAsync(List<CreateBilling> invoices, CancellationToken cancellationToken)
    {

        return Task.FromResult(invoices);
    }
}