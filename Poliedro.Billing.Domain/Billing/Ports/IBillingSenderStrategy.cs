namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingSenderStrategy
{
    Task SendInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken CancellationToken);
}
