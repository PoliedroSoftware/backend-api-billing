namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingSenderStrategy<TDto>
{
    Task SendInvoicesAsync(IEnumerable<TDto> invoices, CancellationToken CancellationToken);
}
