using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingSender
{
    Task<ApiResponseFERetailPos> SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken);
}
