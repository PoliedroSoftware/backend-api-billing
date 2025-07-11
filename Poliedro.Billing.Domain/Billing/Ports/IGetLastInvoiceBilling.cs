using Poliedro.Billing.Domain.Client.Entities;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IGetLastInvoiceBilling
{
    Task<int> GetLastInvoiceNumberAsync(CustomerEntity clientItem, CancellationToken cancellationToken);
}
