namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IBillingSender
{
    Task SendAsync(IEnumerable<object> invoices, CancellationToken cancellationToken);
}
