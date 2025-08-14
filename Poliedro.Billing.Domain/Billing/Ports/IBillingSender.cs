using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingSender
{
    Task<List<ApiResponseFERetailPos>> SendAsync(PlemsiInvoiceRequest request, BillingInfoClient clientInfo, CancellationToken cancellationToken);
}
