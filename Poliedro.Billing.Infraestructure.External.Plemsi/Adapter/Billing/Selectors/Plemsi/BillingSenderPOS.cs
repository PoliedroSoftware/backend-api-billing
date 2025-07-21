using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
public class BillingSenderPOS : IBillingSender
{
    public async Task<ApiResponseFERetailPos> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
