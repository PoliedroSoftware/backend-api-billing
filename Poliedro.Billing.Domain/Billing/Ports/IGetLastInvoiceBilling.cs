
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IGetLastInvoiceBilling
{
    Task<int> GetLastInvoiceNumberAsync(BillingInfoClient clientInfo, CancellationToken cancellationToken);
}
