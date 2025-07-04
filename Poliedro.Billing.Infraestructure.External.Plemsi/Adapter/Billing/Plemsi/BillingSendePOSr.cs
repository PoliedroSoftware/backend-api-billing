namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Plemsi;

using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Billing;
public class BillingSendePOSr: IBillingSenderStrategy
{
    public async Task SendInvoicesAsync(IEnumerable<CreateBilling> Invoices, CancellationToken CancellationToken)
    {

       
    }
}