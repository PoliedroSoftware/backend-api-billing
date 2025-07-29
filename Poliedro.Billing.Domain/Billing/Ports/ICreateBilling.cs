using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;
public interface ICreateBilling
{
    Task<IEnumerable<(CreateBilling Billing, object Output)>> CreateInvoicesAsync(
        IEnumerable<CreateBilling> invoices, ClientEntity clientEntity, CancellationToken cancellationToken);
}