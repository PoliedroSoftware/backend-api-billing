using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.BillingPos.Ports;

public interface IInvoiceLastPos
{
    Task<int> GetInvoiceLastAsync(string connectionString, ClientEntity clientEntity, CancellationToken cancellationToken);
}
