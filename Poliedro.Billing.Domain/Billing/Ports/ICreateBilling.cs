namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling
{
    Task<IEnumerable<CreateBilling>> CreateInvoicesAsync(IEnumerable<CreateBilling> invoices, CancellationToken cancellationToken);
}