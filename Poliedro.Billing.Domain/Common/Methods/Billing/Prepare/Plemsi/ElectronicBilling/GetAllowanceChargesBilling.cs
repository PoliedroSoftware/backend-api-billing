using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;
public class GetAllowanceChargesBilling : IAllowanceChargesBilling
{
    public Task<IEnumerable<AllowanceChargeEntity>> AllowanceChargesBillingAsync(List<ItemElectronicEntity> AllowanceCharges, double TotalDiscount)
    {
        throw new NotImplementedException();
    }
}
