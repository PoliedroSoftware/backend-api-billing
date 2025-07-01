using Poliedro.Billing.Domain.BillingPos;
using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.BillingPos.Ports;

public interface IInvoicePos
{
    Task<IEnumerable<InvoiceEntity>> GetInvoicesAsync(string connectionString, CancellationToken cancellationToken, ClientEntity client);
}
