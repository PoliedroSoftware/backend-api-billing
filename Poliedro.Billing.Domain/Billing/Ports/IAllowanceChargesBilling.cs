
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Billing.Ports;

public interface IAllowanceChargesBilling
{
    Task<IEnumerable<AllowanceChargeEntity>> AllowanceChargesBillingAsync(List<ItemElectronicEntity> AllowanceCharges,double TotalDiscount);
}
