namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBillingStrategy
{
    Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken);
}