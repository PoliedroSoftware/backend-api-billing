using Poliedro.Billing.Domain.Billing;
using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
public class BillingSenderPOS : IBillingSender
{
    public Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, BillingInfoClient clientInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
