using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Plemsi;

public class BillingFESender : IBillingSender
{
    public async Task SendInvoicesAsync(IEnumerable<CreateBilling> Invoices, CancellationToken CancellationToken)
    {

    }
}