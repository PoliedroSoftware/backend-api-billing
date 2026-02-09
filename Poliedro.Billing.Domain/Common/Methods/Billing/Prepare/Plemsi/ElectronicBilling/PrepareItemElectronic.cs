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

                bool isFreeItem =
                    item.LineDiscountType == "percentage" &&
                    item.LineDiscountAmount == 100;

                if (isFreeItem)
                {
                    itemsInvoiceResponse.Add(new ItemElectronicEntity
                    {
                        UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : 70,
                        InvoicedQuantity = item.InvoicedQuantity,
                        FreeOfChargeIndicator = true,

                        LineExtensionAmount = 0,

                        AllowanceCharges = new(),
                        WithHoldingTaxTotal = new(),

                        TaxTotals = new()
                        {
                            new TaxTotalEntity
                            {
                                TaxId = 1,         
                                Percent = 0,
                                TaxAmount = 0,
                                TaxableAmount = 0
                            }
                        },

                        Description = $"{item.Description} (ENTREGA GRATUITA)",
                        Notes = string.IsNullOrWhiteSpace(item.Notes)
                        ? "Description"
                        : item.Notes,
                        Code = item.Code,
                        TypeItemIdentificationId = 1,

                        
                        PriceAmount = Math.Round((double)item.UnitPriceBeforeDiscount, 2),
                        BaseQuantity = item.BaseQuantity
                    });

                    continue;
                }

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

                
                double taxableAmount = Math.Round(item.UnitPrice * item.InvoicedQuantity, 2, MidpointRounding.AwayFromZero);
                double taxAmount = Math.Round(taxableAmount * (item.Percent / 100), 2, MidpointRounding.AwayFromZero);


                var taxTotals = new List<TaxTotalEntity>
            {
                new TaxTotalEntity
                {
                       TaxId = 1,
                        Percent = item.Percent,
                        TaxAmount = taxAmount,
                        TaxableAmount = taxableAmount
                }
            };


                var itemInvoice = new ItemElectronicEntity
                {
                    UnitMeasureId = item.UnitMeasureId > 0 ? item.UnitMeasureId : 70,
                    LineExtensionAmount = taxableAmount,
                    InvoicedQuantity = item.InvoicedQuantity,
                    FreeOfChargeIndicator  = false,
                    AllowanceCharges = allowanceCharges,
                    TaxTotals = taxTotals,
                    WithHoldingTaxTotal = [],
                    Description = $"{item.Description} {(item.TaxTotals?.FirstOrDefault()?.TaxAmount > 0 ? $"IVA {item.TaxTotals?.FirstOrDefault()?.TaxAmount}" : "")}",
                    Notes = string.IsNullOrWhiteSpace(item.Notes)
                    ? "Description"
                    : item.Notes,
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
