using Poliedro.Billing.Domain.BillingPos;

namespace Poliedro.Billing.Domain.BillingPos.Ports;

public interface IGetItemsInvoicePos
{
    Task<List<ItemFERetailEntity>> GetItemsInvoicePosAsync(int invoice, string connectionString);
}
