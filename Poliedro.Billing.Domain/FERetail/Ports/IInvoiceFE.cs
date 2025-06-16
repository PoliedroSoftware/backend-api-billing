using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface IInvoiceFE
{
    Task<IEnumerable<InvoiceEntity>> GetInvoicesAsync(string connectionString, CancellationToken cancellationToken, ClientEntity clientEntity);
}
