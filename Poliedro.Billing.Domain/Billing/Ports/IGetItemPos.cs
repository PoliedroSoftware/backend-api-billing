using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IGetItemPos
{
    Task<List<ItemsInvoiceEntity>> GetItemsAsync(int invoiceItem, string connectionString);

}
