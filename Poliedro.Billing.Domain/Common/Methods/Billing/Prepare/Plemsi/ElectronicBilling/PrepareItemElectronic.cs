using Poliedro.Billing.Domain.Billing.Ports;
using Poliedro.Billing.Domain.FERetail.Entity;

namespace Poliedro.Billing.Domain.Common.Methods.Billing.Prepare.Plemsi.ElectronicBilling
{
    public class PrepareItemElectronic : IPrepareItemBilling
    {
        public async Task<List<ItemElectronicEntity>> PrepareItemBillingAsync(List<ItemElectronicEntity> items)
        {
            var itemsInvoiceResponse = new List<ItemElectronicEntity>();

            foreach (var item in items)
            {
               
                double subtotal = Math.Round((double)(item.PriceAmount * item.InvoicedQuantity), 2, MidpointRounding.AwayFromZero);

                
                double discount = item.LineDiscountAmount > 0 ? Math.Round((double)item.LineDiscountAmount, 2, MidpointRounding.AwayFromZero) : 0;

                
                double taxableBase = Math.Round(subtotal - discount, 2, MidpointRounding.AwayFromZero);

               
                double taxAmount = Math.Round(taxableBase * ((double)item.Percent / 100.0), 2, MidpointRounding.AwayFromZero);

                
                decimal discountRounded = Math.Round((decimal)discount, 2, MidpointRounding.AwayFromZero);
                decimal multiplierFactor = subtotal > 0
                    ? Math.Round((decimal)(discount / subtotal), 6, MidpointRounding.AwayFromZero)
                    : 0m;

               
                var allowanceCharges = new List<AllowanceChargeEntity>();
                if (discount > 0)
                {
                    allowanceCharges.Add(new AllowanceChargeEntity
                    {
                        ChargeIndicator = false,
                        AllowanceChargeReason = "Discount",
                        MultiplierFactorNumeric = multiplierFactor,
                        Amount = discountRounded,
                        BaseAmount = Math.Round((decimal)subtotal, 2, MidpointRounding.AwayFromZero)
                    });
                }

                
                var taxTotals = new List<TaxTotalEntity>
            {
                new TaxTotalEntity
                {
                    TaxId = item.TaxTotals != null && item.TaxTotals.Any() ? item.TaxTotals.First().TaxId : 1,
                    Percent = item.Percent,
                    TaxAmount = taxAmount,
                    TaxableAmount = taxableBase
                }
            };

                
                var itemInvoice = new ItemElectronicEntity
                {
                    UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : 70,
                    LineExtensionAmount = taxableBase,
                    InvoicedQuantity = Math.Round(item.InvoicedQuantity, 2, MidpointRounding.AwayFromZero),
                    FreeOfChargeIndicator = item.FreeOfChargeIndicator,
                    AllowanceCharges = allowanceCharges,
                    TaxTotals = taxTotals,
                    WithHoldingTaxTotal = [],
                    Description = item.Description + (taxAmount > 0 ? $" IVA {taxAmount:F2}" : ""),
                    Notes = item.Notes,
                    Code = item.Code,
                    TypeItemIdentificationId = 1,
                    PriceAmount = Math.Round(item.PriceAmount, 2, MidpointRounding.AwayFromZero),
                    BaseQuantity = item.BaseQuantity,
                };

                itemsInvoiceResponse.Add(itemInvoice);
            }

            return await Task.FromResult(itemsInvoiceResponse);
        }
    }

}
