using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingSender
{
    Task SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken);
}
