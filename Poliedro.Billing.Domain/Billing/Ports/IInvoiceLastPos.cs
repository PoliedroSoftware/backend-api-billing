using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IInvoiceLastPos
{
    Task<int> GetInvoiceLastAsync(string connectionString, ClientEntity clientEntity, CancellationToken cancellationToken);
}
