namespace Poliedro.Billing.Domain.BillingPos.Ports;
public interface ICreateBillingStrategy
{
    Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken);
}