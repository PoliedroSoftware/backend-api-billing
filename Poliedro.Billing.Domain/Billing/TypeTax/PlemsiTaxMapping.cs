using Poliedro.Billing.Domain.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poliedro.Billing.Application.Billing.Services.Selectors.Plemsi.TypeTax;

public static class PlemsiTaxMapping
{
    public static readonly IReadOnlyDictionary<TaxType, int> Ids =
     new Dictionary<TaxType, int>
     {
         [TaxType.IVA] = 1,
         [TaxType.IC] = 2,
         [TaxType.ICA] = 3,
         [TaxType.INC] = 4,
         [TaxType.ReteIVA] = 5,
         [TaxType.ReteRenta] = 6,
         [TaxType.ReteICA] = 7,
         [TaxType.FtoHorticultura] = 8,
         [TaxType.Timbre] = 9,
         [TaxType.INCBolsas] = 10,
         [TaxType.INCarbono] = 11,
         [TaxType.INCombustibles] = 12,
         [TaxType.SobretasaCombustibles] = 13,
         [TaxType.Sordicom] = 14,
         [TaxType.ICPorcentual] = 16,
         [TaxType.ICDatos] = 17,
         [TaxType.IVAEINC] = 18,
         [TaxType.NoAplica] = 19,
         [TaxType.ICL] = 20,
         [TaxType.INPP] = 21,
         [TaxType.IBUA] = 22,
         [TaxType.ICUI] = 23,
         [TaxType.ADV] = 24
     };

    public static int GetPlemsiId(int internalTaxId)
    {
        var taxType = (TaxType)internalTaxId;

        if (!Ids.TryGetValue(taxType, out var plemsiTaxId))
        {
            throw new ArgumentException(
                $"El impuesto interno {internalTaxId} no tiene equivalencia en Plemsi.");
        }

        return plemsiTaxId;
    }
}
