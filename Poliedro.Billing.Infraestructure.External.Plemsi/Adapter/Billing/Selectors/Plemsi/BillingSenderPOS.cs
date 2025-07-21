using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.Common.Methods.Billing.Sender.Plemsi;
namespace Poliedro.Billing.Infraestructure.External.Plemsi.Adapter.Billing.Selectors.Plemsi;
public class BillingSenderPOS : IBillingSender
{
    public async Task SendAsync(PlemsiInvoiceRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();

        await Task.CompletedTask;
    }
}
