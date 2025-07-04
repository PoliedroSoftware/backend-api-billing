
namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingSenderOrchestrator
{
    Task SendInvoicesAsync(IEnumerable<CreateBilling> invoices, string provider, string typeResolution, CancellationToken cancellationToken);
}
