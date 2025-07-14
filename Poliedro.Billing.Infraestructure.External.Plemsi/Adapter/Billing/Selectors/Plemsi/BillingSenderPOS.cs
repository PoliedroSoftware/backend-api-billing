namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi;

public class BillingSenderPOS : IBillingSender<PlemiPOSInvoiceDTO>
{
    public async Task SendInvoicesAsync(IEnumerable<PlemiPOSInvoiceDTO> invoices, CancellationToken cancellationToken)
    {
       
    }
}
