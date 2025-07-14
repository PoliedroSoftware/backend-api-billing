namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling<TInput,TOutput>
{
    Task<IEnumerable<TOutput>> CreateInvoicesAsync(IEnumerable<TInput> invoices, CancellationToken cancellationToken);
}