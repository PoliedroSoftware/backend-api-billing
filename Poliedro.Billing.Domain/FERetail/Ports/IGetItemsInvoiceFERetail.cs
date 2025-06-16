using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.FERetail.Ports;

public interface IGetItemsInvoiceFERetail
{
    Task<List<ItemElectronicEntity>> GetItemsInvoiceFERetailAsync(string invoice, string connectionString);
}

