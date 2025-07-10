using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;
using Poliedro.Billing.Domain.Siigo.Models;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare;
public class GetAllTaxTotalsBilling : IGetAllTaxTotalsBilling
{
    public async Task<List<AllTaxTotalEntity>> IGetAllTaxTotalsBillingAsync(List<ItemElectronicEntity> AllTaxTotal)
    {
        var allTaxTotals = new List<AllTaxTotalEntity>();

        foreach (var item in AllTaxTotal)
        {
            foreach (var tax in item.TaxTotals)
            {
                allTaxTotals.Add(new AllTaxTotalEntity
                {
                    TaxId = tax.TaxId,
                    TaxAmount = tax.TaxAmount,
                    Percent = tax.Percent,
                    TaxableAmount = tax.TaxableAmount
                });
            }
        }

        return await Task.FromResult(allTaxTotals);
    }
}
