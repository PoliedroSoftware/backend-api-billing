using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling;
public class GetAllTaxTotalsBilling : IGetAllTaxTotalsBilling
{
    public async Task<List<AllTaxTotalEntity>> IGetAllTaxTotalsBillingAsync(List<ItemElectronicEntity> AllTaxTotal)
    {
        var groupedTaxes = new Dictionary<(int TaxId, double Percent), (double TaxAmount, double TaxableAmount)>();

        foreach (var item in AllTaxTotal)
        {
            if (item.TaxTotals == null) continue;

            foreach (var tax in item.TaxTotals)
            {
                var key = (tax.TaxId, tax.Percent);

                if (groupedTaxes.ContainsKey(key))
                {
                    var existing = groupedTaxes[key];
                    groupedTaxes[key] = (
                        existing.TaxAmount + tax.TaxAmount,
                        existing.TaxableAmount + tax.TaxableAmount
                    );
                }
                else
                {
                    groupedTaxes[key] = (tax.TaxAmount, tax.TaxableAmount);
                }
            }
        }

        var allTaxTotals = groupedTaxes.Select(kvp => new AllTaxTotalEntity
        {
            TaxId = kvp.Key.TaxId,
            Percent = kvp.Key.Percent,
            TaxAmount = Math.Round(kvp.Value.TaxAmount, 2, MidpointRounding.AwayFromZero),
            TaxableAmount = Math.Round(kvp.Value.TaxableAmount, 2, MidpointRounding.AwayFromZero)
        }).ToList();

        return await Task.FromResult(allTaxTotals);
    }
}
