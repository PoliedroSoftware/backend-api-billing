using Poliedro.Billing.Domain.Billing.Ports;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
public class BillingSenderPOS : IBillingSender
{
    public Task SendAsync(IEnumerable<object> invoices, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
