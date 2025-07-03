namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Plemsi;

using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.BillingPos;
public class BillingPOSSender : IBillingSender
{
    public async Task SendInvoicesAsync(IEnumerable<CreateBilling> Invoices, CancellationToken CancellationToken)
    {

       
    }
}