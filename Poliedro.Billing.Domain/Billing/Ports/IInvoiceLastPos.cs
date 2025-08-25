using Poliedro.Billing.Domain.Client.Entities;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IInvoiceLastPos
{
    Task<int> GetInvoiceLastAsync(BillingInfoClient clientInfo, CancellationToken cancellationToken);
}
