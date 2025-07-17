namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling
{
    Task<IEnumerable<object>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken);
}