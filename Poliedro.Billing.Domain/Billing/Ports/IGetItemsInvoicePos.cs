using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IGetItemsInvoicePos
{
    Task<List<ItemFERetailEntity>> GetItemsInvoicePosAsync(int invoice, string connectionString);
}
