using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi;

public class PrepareBillingPOS : ICreateBilling //POS
{
    public Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, BillingInfoClient clientInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}