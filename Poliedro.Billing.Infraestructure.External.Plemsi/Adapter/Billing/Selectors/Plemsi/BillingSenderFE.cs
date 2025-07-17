using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

public class BillingSenderFE : IBillingSender<PlemiFEInvoiceDTO>
{
    public Task SendInvoicesAsync(IEnumerable<PlemiFEInvoiceDTO> invoices, CancellationToken CancellationToken)
    {
        throw new NotImplementedException();
    }
}