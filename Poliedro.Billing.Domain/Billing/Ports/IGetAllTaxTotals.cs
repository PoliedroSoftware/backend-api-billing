using Poliedro.Billing.Domain.FERetail.Entity;
namespace Poliedro.Billing.Domain.Billing.Ports;
public interface IGetAllTaxTotalsBilling
{
    Task<List<AllTaxTotalEntity>> IGetAllTaxTotalsBillingAsync(List<ItemElectronicEntity> AllTaxTotal);
}
