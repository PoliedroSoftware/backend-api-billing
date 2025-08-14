using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IPrepareItemBilling
{
    Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items);
}
