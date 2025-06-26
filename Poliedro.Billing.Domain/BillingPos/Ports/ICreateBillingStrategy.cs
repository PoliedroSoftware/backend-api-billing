namespace Poliedro.Billing.Domain.BillingPos.Ports;
public interface ICreateBillingStrategy
{
    Task<List<CreateBilling>> CreateInvoicesAsync(List<CreateBilling> invoices, CancellationToken cancellationToken);
}