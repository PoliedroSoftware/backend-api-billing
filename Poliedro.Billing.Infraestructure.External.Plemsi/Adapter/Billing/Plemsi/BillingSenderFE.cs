using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Plemsi;

public class BillingSenderFE: IBillingSenderStrategy
{
    public async Task SendInvoicesAsync(IEnumerable<CreateBilling> Invoices, CancellationToken CancellationToken)
    {

    }
}