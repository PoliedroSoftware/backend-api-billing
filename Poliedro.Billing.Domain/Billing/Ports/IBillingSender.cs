namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IBillingSender<TDto>
{
    Task SendInvoicesAsync(IEnumerable<TDto> invoices, CancellationToken CancellationToken);
}
