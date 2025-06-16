using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface IGetItemFE
{
    Task<List<ItemsInvoiceEntity>> GetItemsAsync(string invoiceItem, string connectionString);
}

