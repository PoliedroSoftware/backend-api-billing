

using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare;

public class PrepareItemElectronic
{
    public List<ItemElectronicEntity> UpdateItem(List<ItemElectronicEntity> items)
    {


        foreach (var item in items)
        {

            List<ItemElectronicEntity> Itens = null;

            double totalToBase = Itens?.Sum(item => item.LineExtensionAmount) ?? 0;


            Itens = new List<ItemElectronicEntity>
            {
                new ItemElectronicEntity { LineExtensionAmount = 100 },
                new ItemElectronicEntity { LineExtensionAmount = 200 }
            };

            totalToBase = Itens?.Sum(item => item.LineExtensionAmount) ?? 0;
            double totalTaxableAmount = item.TaxTotals?.Sum(tax => tax.TaxAmount) ?? 0;
            double totalBaseGravable = item.TaxTotals?.Sum(tax => tax.TaxableAmount) ?? 0;
            double totalToPay = totalToBase + totalTaxableAmount;


        }

        return items;
    }

}
