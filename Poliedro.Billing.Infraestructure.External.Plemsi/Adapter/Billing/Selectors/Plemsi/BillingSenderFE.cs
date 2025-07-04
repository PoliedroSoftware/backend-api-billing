using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Application.Billing.Dtos.Plemsi;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;

public class BillingSenderFE: IBillingSenderStrategy<PlemiFEInvoiceDTO>
{
    public async Task SendInvoicesAsync(IEnumerable<PlemiFEInvoiceDTO> Invoices, CancellationToken CancellationToken)
    {

    }
}