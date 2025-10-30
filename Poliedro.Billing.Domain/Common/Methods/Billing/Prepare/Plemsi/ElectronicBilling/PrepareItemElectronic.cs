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

                double TotalGross = (double)(item.UnitPriceBeforeDiscount * item.InvoicedQuantity);

                double subtotal = item.UnitPrice * item.InvoicedQuantity;

                double DiscountAmount = TotalGross - subtotal;

                double multiplierFactor = Math.Round(subtotal / TotalGross, 5, MidpointRounding.AwayFromZero);


                var allowanceCharges = new List<AllowanceChargeEntity>();
                if (DiscountAmount > 0)
                {
                    allowanceCharges.Add(new AllowanceChargeEntity
                    {
                        ChargeIndicator = false,
                        AllowanceChargeReason = "Discount",
                        MultiplierFactorNumeric = (decimal)multiplierFactor,
                        Amount = (decimal)DiscountAmount,
                        BaseAmount = Math.Round((decimal)subtotal, 2, MidpointRounding.AwayFromZero)
                    });
                }


                var taxTotals = new List<TaxTotalEntity>
            {
                new TaxTotalEntity
                {
                       TaxId = 1,
                        Percent = item.Percent,
                        TaxAmount = item.TaxAmount * item.InvoicedQuantity,
                        TaxableAmount = item.UnitPrice * item.InvoicedQuantity
                }
            };


                var itemInvoice = new ItemElectronicEntity
                {
                    UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : 70,
                    LineExtensionAmount = item.UnitPrice * item.InvoicedQuantity,
                    InvoicedQuantity = item.InvoicedQuantity,
                    FreeOfChargeIndicator = item.FreeOfChargeIndicator,
                    AllowanceCharges = allowanceCharges,
                    TaxTotals = taxTotals,
                    WithHoldingTaxTotal = [],
                    Description = $"{item.Description} {(item.TaxTotals?.FirstOrDefault()?.TaxAmount > 0 ? $"IVA {item.TaxTotals?.FirstOrDefault()?.TaxAmount}" : "")}",
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
